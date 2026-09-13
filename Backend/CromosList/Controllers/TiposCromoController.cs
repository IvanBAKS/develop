using CromosList.Data;
using CromosList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TiposCromoController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public TiposCromoController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tiposCromo = await _context.TiposCromo
            .AsNoTracking()
            .ToListAsync();

        return Ok(tiposCromo);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var tipoCromo = await _context.TiposCromo
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tipoCromo is null)
            return NotFound();

        return Ok(tipoCromo);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] TipoCromo tipoCromo)
    {
        if (string.IsNullOrWhiteSpace(tipoCromo.Nombre))
        {
            return BadRequest("El nombre del tipo de cromo es obligatorio.");
        }

        _context.TiposCromo.Add(tipoCromo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = tipoCromo.Id }, tipoCromo);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Editar(long id, [FromBody] TipoCromo tipoCromo)
    {
        var existente = await _context.TiposCromo.FindAsync(id);

        if (existente is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(tipoCromo.Nombre))
        {
            return BadRequest("El nombre del tipo de cromo es obligatorio.");
        }

        existente.Nombre = tipoCromo.Nombre;
        await _context.SaveChangesAsync();

        return Ok(existente);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var tipoCromo = await _context.TiposCromo.FindAsync(id);

        if (tipoCromo is null)
            return NotFound();

        _context.TiposCromo.Remove(tipoCromo);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}