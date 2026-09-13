using CromosList.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CromosList.Dtos;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CromoController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public CromoController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? q)
    {
        var consulta = _context.Cromos
            .AsNoTracking()
            .Include(c => c.Jugador)
            .Include(c => c.Equipo)
            .Include(c => c.Edicion)
            .Include(c => c.Publicacion)
            .Include(c => c.TipoCromo)
            .Include(c => c.Coleccion)
            .Include(c => c.Album)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var termino = q.Trim();
            consulta = consulta.Where(c =>
                c.Jugador.Nombre.ToLower().Contains(termino.ToLower())
                || c.Jugador.NombreCompleto.ToLower().Contains(termino.ToLower())
                || c.Equipo.Nombre.ToLower().Contains(termino.ToLower()));
        }

        var cromos = await consulta
            .OrderBy(c => c.Jugador.Nombre)
            .Select(c => new
            {
                c.Id,
                c.Numero,

                Jugador = new
                {
                    c.Jugador.Id,
                    c.Jugador.Nombre,
                    c.Jugador.NombreCompleto
                },

                Equipo = new
                {
                    c.Equipo.Id,
                    c.Equipo.Nombre
                },

                Edicion = new
                {
                    c.Edicion.Id,
                    c.Edicion.Nombre
                },

                Publicacion = c.Publicacion == null
                    ? null
                    : new
                    {
                        c.Publicacion.Id,
                        c.Publicacion.Temporada,
                        c.Publicacion.Nombre,
                        c.Publicacion.FechaPublicacion
                    },

                TipoCromo = new
                {
                    c.TipoCromo.Id,
                    c.TipoCromo.Nombre
                },

                Coleccion = c.Coleccion == null
                    ? null
                    : new
                    {
                        c.Coleccion.Id,
                        c.Coleccion.Nombre
                    },

                Album = new
                {
                    c.Album.Id,
                    c.Album.Nombre,
                    c.Album.Temporada
                }
            })
            .ToListAsync();

        return Ok(cromos);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var cromo = await _context.Cromos
            .AsNoTracking()
            .Include(c => c.Jugador)
            .Include(c => c.Equipo)
            .Include(c => c.Edicion)
            .Include(c => c.Publicacion)
            .Include(c => c.TipoCromo)
            .Include(c => c.Coleccion)
            .Include(c => c.Album)
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Numero,

                Jugador = new
                {
                    c.Jugador.Id,
                    c.Jugador.Nombre,
                    c.Jugador.NombreCompleto
                },

                Equipo = new
                {
                    c.Equipo.Id,
                    c.Equipo.Nombre
                },

                Edicion = new
                {
                    c.Edicion.Id,
                    c.Edicion.Nombre
                },

                Publicacion = c.Publicacion == null
                    ? null
                    : new
                    {
                        c.Publicacion.Id,
                        c.Publicacion.Temporada,
                        c.Publicacion.Nombre,
                        c.Publicacion.FechaPublicacion
                    },

                TipoCromo = new
                {
                    c.TipoCromo.Id,
                    c.TipoCromo.Nombre
                },

                Coleccion = c.Coleccion == null
                    ? null
                    : new
                    {
                        c.Coleccion.Id,
                        c.Coleccion.Nombre
                    },

                Album = new
                {
                    c.Album.Id,
                    c.Album.Nombre,
                    c.Album.Temporada
                }
            })
            .FirstOrDefaultAsync();

        if (cromo is null)
            return NotFound();

        return Ok(cromo);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCromoDto dto)
    {
        var jugadorExiste = await _context.Jugadores
            .AnyAsync(j => j.Id == dto.JugadorId);

        if (!jugadorExiste)
            return BadRequest("El jugador no existe.");

        var equipoExiste = await _context.Equipos
            .AnyAsync(e => e.Id == dto.EquipoId);

        if (!equipoExiste)
            return BadRequest("El equipo no existe.");

        var edicionExiste = await _context.Ediciones
            .AnyAsync(e => e.Id == dto.EdicionId);

        if (!edicionExiste)
            return BadRequest("La edición no existe.");

        var publicacionExiste = await _context.Publicaciones
            .AnyAsync(p => p.Id == dto.PublicacionId);

        if (dto.PublicacionId.HasValue && !publicacionExiste)
            return BadRequest("La publicación no existe.");

        var tipoCromoExiste = await _context.TiposCromo
            .AnyAsync(t => t.Id == dto.TipoCromoId);

        if (!tipoCromoExiste)
            return BadRequest("El tipo de cromo no existe.");

        var coleccionExiste = await _context.Colecciones
            .AnyAsync(c => c.Id == dto.ColeccionId);

        if (dto.ColeccionId.HasValue && !coleccionExiste)
            return BadRequest("La colección no existe.");

        var albumExiste = await _context.Albums
            .AnyAsync(a => a.Id == dto.AlbumId);

        if (!albumExiste)
            return BadRequest("El álbum no existe.");

        var cromo = new Models.Cromo
        {
            Numero = dto.Numero,
            JugadorId = dto.JugadorId,
            EquipoId = dto.EquipoId,
            EdicionId = dto.EdicionId,
            PublicacionId = dto.PublicacionId,
            TipoCromoId = dto.TipoCromoId,
            ColeccionId = dto.ColeccionId,
            AlbumId = dto.AlbumId
        };

        _context.Cromos.Add(cromo);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = cromo.Id },
            cromo);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Editar(long id, [FromBody] CreateCromoDto dto)
    {
        var cromo = await _context.Cromos.FindAsync(id);

        if (cromo is null)
            return NotFound();

        var jugadorExiste = await _context.Jugadores
            .AnyAsync(j => j.Id == dto.JugadorId);

        if (!jugadorExiste)
            return BadRequest("El jugador no existe.");

        var equipoExiste = await _context.Equipos
            .AnyAsync(e => e.Id == dto.EquipoId);

        if (!equipoExiste)
            return BadRequest("El equipo no existe.");

        var edicionExiste = await _context.Ediciones
            .AnyAsync(e => e.Id == dto.EdicionId);

        if (!edicionExiste)
            return BadRequest("La edición no existe.");

        var publicacionExiste = await _context.Publicaciones
            .AnyAsync(p => p.Id == dto.PublicacionId);

        if (dto.PublicacionId.HasValue && !publicacionExiste)
            return BadRequest("La publicación no existe.");

        var tipoCromoExiste = await _context.TiposCromo
            .AnyAsync(t => t.Id == dto.TipoCromoId);

        if (!tipoCromoExiste)
            return BadRequest("El tipo de cromo no existe.");

        var coleccionExiste = await _context.Colecciones
            .AnyAsync(c => c.Id == dto.ColeccionId);

        if (dto.ColeccionId.HasValue && !coleccionExiste)
            return BadRequest("La colección no existe.");

        var albumExiste = await _context.Albums
            .AnyAsync(a => a.Id == dto.AlbumId);

        if (!albumExiste)
            return BadRequest("El álbum no existe.");

        cromo.Numero = dto.Numero;
        cromo.JugadorId = dto.JugadorId;
        cromo.EquipoId = dto.EquipoId;
        cromo.EdicionId = dto.EdicionId;
        cromo.PublicacionId = dto.PublicacionId;
        cromo.TipoCromoId = dto.TipoCromoId;
        cromo.ColeccionId = dto.ColeccionId;
        cromo.AlbumId = dto.AlbumId;

        await _context.SaveChangesAsync();

        return Ok(cromo);
    }
}