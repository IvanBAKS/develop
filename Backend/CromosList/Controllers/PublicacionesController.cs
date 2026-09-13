using CromosList.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PublicacionesController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public PublicacionesController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var publicaciones = await _context.Publicaciones
            .AsNoTracking()
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

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var publicacion = await _context.Publicaciones
            .AsNoTracking()
            .Include(p => p.Edicion)
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.Temporada,
                p.Nombre,
                p.FechaPublicacion,
                Edicion = new
                {
                    p.Edicion.Id,
                    p.Edicion.Nombre
                }
            })
            .FirstOrDefaultAsync();

        if (publicacion is null)
            return NotFound();

        return Ok(publicacion);
    }
}