using CromosList.Data;
using CromosList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EditorialesController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public EditorialesController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var editoriales = await _context.Editoriales
            .AsNoTracking()
            .ToListAsync();

        return Ok(editoriales);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var editorial = await _context.Editoriales
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (editorial is null)
            return NotFound();

        return Ok(editorial);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Editorial editorial)
    {
        if (string.IsNullOrWhiteSpace(editorial.Nombre))
        {
            return BadRequest("El nombre de la editorial es obligatorio.");
        }

        _context.Editoriales.Add(editorial);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = editorial.Id }, editorial);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Editar(long id, [FromBody] Editorial editorial)
    {
        var existente = await _context.Editoriales.FindAsync(id);

        if (existente is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(editorial.Nombre))
        {
            return BadRequest("El nombre de la editorial es obligatorio.");
        }

        existente.Nombre = editorial.Nombre;
        await _context.SaveChangesAsync();

        return Ok(existente);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var editorial = await _context.Editoriales.FindAsync(id);

        if (editorial is null)
            return NotFound();

        _context.Editoriales.Remove(editorial);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
