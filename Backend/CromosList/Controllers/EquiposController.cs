using CromosList.Data;
using CromosList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquiposController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public EquiposController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var equipos = await _context.Equipos
            .AsNoTracking()
            .ToListAsync();

        return Ok(equipos);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Equipo equipo)
    {
        if (string.IsNullOrWhiteSpace(equipo.Nombre))
        {
            return BadRequest("El nombre del equipo es obligatorio.");
        }

        _context.Equipos.Add(equipo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = equipo.Id }, equipo);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Editar(long id, [FromBody] Equipo equipo)
    {
        var existente = await _context.Equipos.FindAsync(id);

        if (existente is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(equipo.Nombre))
        {
            return BadRequest("El nombre del equipo es obligatorio.");
        }

        existente.Nombre = equipo.Nombre;
        await _context.SaveChangesAsync();

        return Ok(existente);
    }

    [HttpGet("{id:long}/Jugadores")]
    public async Task<IActionResult> GetJugadores(long id)
    {
        var equipoExiste = await _context.Equipos
            .AnyAsync(e => e.Id == id);

        if (!equipoExiste)
            return NotFound();

        var jugadores = await _context.Jugadores
            .AsNoTracking()
            .Where(j => j.Equipos.Any(e => e.Id == id))
            .Select(j => new
            {
                j.Id,
                j.Nombre,
                j.NombreCompleto
            })
            .OrderBy(j => j.Nombre)
            .ToListAsync();

        return Ok(jugadores);
    }
}