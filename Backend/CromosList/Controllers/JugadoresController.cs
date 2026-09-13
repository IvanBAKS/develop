using CromosList.Data;
using CromosList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JugadoresController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public JugadoresController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? equipoId)
    {
        var query = _context.Jugadores
            .AsNoTracking()
            .AsQueryable();

        if (equipoId.HasValue)
        {
            query = query.Where(j => j.Equipos.Any(e => e.Id == equipoId.Value));
        }

        var jugadores = await query
            .OrderBy(j => j.Nombre)
            .Select(j => new
            {
                j.Id,
                j.Nombre,
                j.NombreCompleto,
                Equipos = j.Equipos
                    .OrderBy(e => e.Nombre)
                    .Select(e => new
                    {
                        e.Id,
                        e.Nombre
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(jugadores);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var jugador = await _context.Jugadores
            .AsNoTracking()
            .Where(j => j.Id == id)
            .Select(j => new
            {
                j.Id,
                j.Nombre,
                j.NombreCompleto,
                Equipos = j.Equipos
                    .OrderBy(e => e.Nombre)
                    .Select(e => new
                    {
                        e.Id,
                        e.Nombre
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (jugador is null)
            return NotFound();

        return Ok(jugador);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] JugadorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest("El nombre del jugador es obligatorio.");

        var equipos = await ValidarEquipos(dto.Equipos);
        if (equipos is null)
            return BadRequest("Uno o más equipos especificados no existen.");

        var jugador = new Jugador
        {
            Nombre = dto.Nombre.Trim(),
            NombreCompleto = dto.NombreCompleto?.Trim() ?? string.Empty,
            Equipos = equipos
        };

        _context.Jugadores.Add(jugador);
        await _context.SaveChangesAsync();

        var creado = await Proyeccion(jugador.Id);
        return CreatedAtAction(nameof(GetById), new { id = jugador.Id }, creado);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Editar(long id, [FromBody] JugadorDto dto)
    {
        var existente = await _context.Jugadores
            .Include(j => j.Equipos)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (existente is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest("El nombre del jugador es obligatorio.");

        var equipos = await ValidarEquipos(dto.Equipos);
        if (equipos is null)
            return BadRequest("Uno o más equipos especificados no existen.");

        existente.Nombre = dto.Nombre.Trim();
        existente.NombreCompleto = dto.NombreCompleto?.Trim() ?? string.Empty;
        existente.Equipos = equipos;

        await _context.SaveChangesAsync();

        return Ok(await Proyeccion(existente.Id));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var jugador = await _context.Jugadores.FindAsync(id);

        if (jugador is null)
            return NotFound();

        _context.Jugadores.Remove(jugador);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private Task<object?> Proyeccion(long id)
    {
        return _context.Jugadores
            .AsNoTracking()
            .Where(j => j.Id == id)
            .Select(j => new
            {
                j.Id,
                j.Nombre,
                j.NombreCompleto,
                Equipos = j.Equipos
                    .OrderBy(e => e.Nombre)
                    .Select(e => new
                    {
                        e.Id,
                        e.Nombre
                    })
                    .ToList()
            })
            .Cast<object?>()
            .FirstOrDefaultAsync();
    }

    private async Task<List<Equipo>?> ValidarEquipos(IReadOnlyCollection<long>? equipoIds)
    {
        if (equipoIds is null || equipoIds.Count == 0)
            return null;

        var ids = equipoIds.Distinct().ToList();
        var equipos = await _context.Equipos
            .Where(e => ids.Contains(e.Id))
            .ToListAsync();

        if (equipos.Count != ids.Count)
            return null;

        return equipos;
    }
}

public class JugadorDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? NombreCompleto { get; set; }
    public List<long> Equipos { get; set; } = new();
}
