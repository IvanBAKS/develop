using System.Security.Claims;
using CromosList.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public UsuariosController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _context.Usuarios
            .AsNoTracking()
            .Select(u => new { u.Id, u.Nombre, u.Email, u.EsAdmin })
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new { u.Id, u.Nombre, u.Email, u.EsAdmin })
            .FirstOrDefaultAsync();

        if (usuario is null)
            return NotFound();

        return Ok(usuario);
    }

    [HttpPut("{id:long}/admin")]
    public async Task<IActionResult> PromoverAdmin(long id)
    {
        var miId = ObtenerMiId();
        var yo = await _context.Usuarios.FindAsync(miId);

        if (yo is null || !yo.EsAdmin)
            return Forbid();

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null)
            return NotFound();

        usuario.EsAdmin = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id:long}/admin/revocar")]
    public async Task<IActionResult> RevocarAdmin(long id)
    {
        var miId = ObtenerMiId();
        var yo = await _context.Usuarios.FindAsync(miId);

        if (yo is null || !yo.EsAdmin)
            return Forbid();

        if (miId == id)
            return BadRequest(new { message = "No puedes revocar tu propio permiso de administrador." });

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null)
            return NotFound();

        usuario.EsAdmin = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private long ObtenerMiId()
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? User.FindFirst("sub")?.Value;

        return long.TryParse(sub, out var id) ? id : 0;
    }
}
