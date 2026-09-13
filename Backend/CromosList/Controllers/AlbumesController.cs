using CromosList.Data;
using CromosList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlbumesController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public AlbumesController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var albums = await Proyeccion()
            .ToListAsync();

        return Ok(albums);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var album = await Proyeccion(id)
            .FirstOrDefaultAsync();

        if (album is null)
            return NotFound();

        return Ok(album);
    }

    // Detalle "ver todo": incluye los cromos de cada equipo del álbum.
    [HttpGet("{id:long}/Cromos")]
    public async Task<IActionResult> GetCromosPorEquipo(long id)
    {
        var titulo = await _context.Albums
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new { a.Id, a.Nombre, a.Temporada })
            .FirstOrDefaultAsync();

        if (titulo is null)
            return NotFound();

        var cromos = await _context.Cromos
            .AsNoTracking()
            .Include(c => c.Jugador)
            .Include(c => c.Equipo)
            .Include(c => c.TipoCromo)
            .Include(c => c.Edicion)
            .Where(c => c.AlbumId == id)
            .ToListAsync();

        var cromosPorEquipo = cromos
            .GroupBy(c => c.EquipoId)
            .ToDictionary(g => g.Key, g => g
                .Select(c => new
                {
                    c.Id,
                    c.Numero,
                    Jugador = new { c.Jugador.Id, c.Jugador.Nombre },
                    Equipo = new { c.Equipo.Id, c.Equipo.Nombre },
                    Edicion = new { c.Edicion.Id, c.Edicion.Nombre },
                    TipoCromo = new { c.TipoCromo.Id, c.TipoCromo.Nombre }
                })
                .OrderBy(c => c.Numero)
                .ToList());

        var equipos = await _context.AlbumEquipos
            .AsNoTracking()
            .Include(ae => ae.Equipo)
            .Where(ae => ae.AlbumId == id)
            .OrderBy(ae => ae.Orden)
            .Select(ae => new
            {
                ae.Equipo.Id,
                ae.Equipo.Nombre,
                ae.Orden
            })
            .ToListAsync();

        var resultado = equipos.Select(e => new
        {
            e.Id,
            e.Nombre,
            e.Orden,
            Cromos = cromosPorEquipo.TryGetValue(e.Id, out var lista)
                ? lista.Cast<object>()
                : Enumerable.Empty<object>()
        });

        return Ok(new
        {
            titulo.Id,
            titulo.Nombre,
            titulo.Temporada,
            Equipos = resultado
        });
    }

    // Proyección plana y segura para la serialización (evita ciclos objeto).
    private IQueryable<object> Proyeccion(long? id = null)
    {
        IQueryable<Album> consulta = _context.Albums
            .AsNoTracking()
            .Include(a => a.Editorial)
            .Include(a => a.AlbumEquipos).ThenInclude(ae => ae.Equipo)
            .Include(a => a.AlbumTiposCromo).ThenInclude(at => at.TipoCromo)
            .Include(a => a.Cromos);

        if (id.HasValue)
            consulta = consulta.Where(a => a.Id == id.Value);

        return consulta
            .Select(a => new
            {
                a.Id,
                a.Nombre,
                a.Temporada,
                Editorial = new { a.Editorial.Id, a.Editorial.Nombre },
                Equipos = a.AlbumEquipos
                    .OrderBy(ae => ae.Orden)
                    .Select(ae => new { ae.Equipo.Id, ae.Equipo.Nombre, ae.Orden })
                    .ToList(),
                TiposCromo = a.AlbumTiposCromo
                    .OrderBy(at => at.Orden)
                    .Select(at => new { at.TipoCromo.Id, at.TipoCromo.Nombre, at.Orden })
                    .ToList(),
                ContadorCromos = a.Cromos.Count
            });
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AlbumDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest("El nombre del álbum es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Temporada))
            return BadRequest("La temporada es obligatoria.");

        if (!await _context.Editoriales.AnyAsync(e => e.Id == dto.EditorialId))
            return BadRequest("La editorial especificada no existe.");

        if (dto.Equipos is null || dto.Equipos.Count == 0)
            return BadRequest("Debe añadir al menos un equipo.");

        if (dto.TiposCromo is null || dto.TiposCromo.Count == 0)
            return BadRequest("Debe añadir al menos un tipo de cromo.");

        var album = new Album
        {
            Nombre = dto.Nombre,
            Temporada = dto.Temporada,
            EditorialId = dto.EditorialId
        };

        _context.Albums.Add(album);
        await _context.SaveChangesAsync();

        for (int i = 0; i < dto.Equipos.Count; i++)
        {
            var eq = dto.Equipos[i];
            if (!await _context.Equipos.AnyAsync(e => e.Id == eq.EquipoId))
                return BadRequest($"El equipo con id {eq.EquipoId} no existe.");

            _context.AlbumEquipos.Add(new AlbumEquipo
            {
                AlbumId = album.Id,
                EquipoId = eq.EquipoId,
                Orden = i
            });
        }

        for (int i = 0; i < dto.TiposCromo.Count; i++)
        {
            var tc = dto.TiposCromo[i];
            if (!await _context.TiposCromo.AnyAsync(t => t.Id == tc.TipoCromoId))
                return BadRequest($"El tipo de cromo con id {tc.TipoCromoId} no existe.");

            _context.AlbumTiposCromo.Add(new AlbumTipoCromo
            {
                AlbumId = album.Id,
                TipoCromoId = tc.TipoCromoId,
                Orden = i
            });
        }

        await _context.SaveChangesAsync();

        var nuevoAlbum = await Proyeccion(album.Id).FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetById), new { id = album.Id }, nuevoAlbum);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Editar(long id, [FromBody] AlbumDto dto)
    {
        var existente = await _context.Albums
            .Include(a => a.AlbumEquipos)
            .Include(a => a.AlbumTiposCromo)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (existente is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest("El nombre del álbum es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Temporada))
            return BadRequest("La temporada es obligatoria.");

        if (!await _context.Editoriales.AnyAsync(e => e.Id == dto.EditorialId))
            return BadRequest("La editorial especificada no existe.");

        if (dto.Equipos is null || dto.Equipos.Count == 0)
            return BadRequest("Debe añadir al menos un equipo.");

        if (dto.TiposCromo is null || dto.TiposCromo.Count == 0)
            return BadRequest("Debe añadir al menos un tipo de cromo.");

        existente.Nombre = dto.Nombre;
        existente.Temporada = dto.Temporada;
        existente.EditorialId = dto.EditorialId;

        _context.AlbumEquipos.RemoveRange(existente.AlbumEquipos);
        _context.AlbumTiposCromo.RemoveRange(existente.AlbumTiposCromo);

        for (int i = 0; i < dto.Equipos.Count; i++)
        {
            var eq = dto.Equipos[i];
            if (!await _context.Equipos.AnyAsync(e => e.Id == eq.EquipoId))
                return BadRequest($"El equipo con id {eq.EquipoId} no existe.");

            existente.AlbumEquipos.Add(new AlbumEquipo
            {
                AlbumId = id,
                EquipoId = eq.EquipoId,
                Orden = i
            });
        }

        for (int i = 0; i < dto.TiposCromo.Count; i++)
        {
            var tc = dto.TiposCromo[i];
            if (!await _context.TiposCromo.AnyAsync(t => t.Id == tc.TipoCromoId))
                return BadRequest($"El tipo de cromo con id {tc.TipoCromoId} no existe.");

            existente.AlbumTiposCromo.Add(new AlbumTipoCromo
            {
                AlbumId = id,
                TipoCromoId = tc.TipoCromoId,
                Orden = i
            });
        }

        await _context.SaveChangesAsync();

        var albumActualizado = await Proyeccion(id).FirstOrDefaultAsync();

        return Ok(albumActualizado);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var album = await _context.Albums.FindAsync(id);

        if (album is null)
            return NotFound();

        _context.Albums.Remove(album);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class AlbumDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Temporada { get; set; } = string.Empty;
    public long EditorialId { get; set; }
    public List<AlbumEquipoDto> Equipos { get; set; } = new();
    public List<AlbumTipoCromoDto> TiposCromo { get; set; } = new();
}

public class AlbumEquipoDto
{
    public long EquipoId { get; set; }
}

public class AlbumTipoCromoDto
{
    public long TipoCromoId { get; set; }
}
