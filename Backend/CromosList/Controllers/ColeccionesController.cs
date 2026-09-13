using CromosList.Data;
using CromosList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ColeccionesController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public ColeccionesController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var colecciones = await _context.Colecciones
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        return Ok(colecciones);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var nombre = await _context.Colecciones
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => c.Nombre)
            .FirstOrDefaultAsync();

        if (nombre is null)
            return NotFound();

        var cromos = await _context.Cromos
            .AsNoTracking()
            .Include(c => c.Equipo)
            .Include(c => c.Jugador)
            .Include(c => c.TipoCromo)
            .Where(c => c.ColeccionId == id)
            .ToListAsync();

        var equipos = cromos
            .GroupBy(c => new { c.Equipo.Id, c.Equipo.Nombre })
            .Select(g => new
            {
                g.Key.Id,
                g.Key.Nombre,
                Cromos = g.Select(c => new
                {
                    c.Id,
                    c.Numero,
                    Jugador = new { c.Jugador.Id, c.Jugador.Nombre },
                    TipoCromo = new { c.TipoCromo.Id, c.TipoCromo.Nombre }
                }).OrderBy(c => c.Numero).ToList()
            })
            .OrderBy(e => e.Nombre)
            .ToList();

        return Ok(new
        {
            Id = id,
            Nombre = nombre,
            Equipos = equipos
        });
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Coleccion coleccion)
    {
        if (string.IsNullOrWhiteSpace(coleccion.Nombre))
        {
            return BadRequest("El nombre de la colección es obligatorio.");
        }

        _context.Colecciones.Add(coleccion);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = coleccion.Id }, coleccion);
    }
}
