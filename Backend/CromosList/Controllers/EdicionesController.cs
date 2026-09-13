using CromosList.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EdicionesController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public EdicionesController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ediciones = await _context.Ediciones
            .AsNoTracking()
            .ToListAsync();

        return Ok(ediciones);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var edicion = await _context.Ediciones
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (edicion is null)
            return NotFound();

        return Ok(edicion);
    }

    [HttpGet("{id:long}/Publicaciones")]
    public async Task<IActionResult> GetPublicaciones(long id)
    {
        var edicionExiste = await _context.Ediciones
            .AnyAsync(e => e.Id == id);

        if (!edicionExiste)
            return NotFound();

        var publicaciones = await _context.Publicaciones
            .AsNoTracking()
            .Where(p => p.EdicionId == id)
            .Select(p => new
            {
                p.Id,
                p.Temporada,
                p.Nombre,
                p.FechaPublicacion
            })
            .ToListAsync();

        return Ok(publicaciones);
    }
}