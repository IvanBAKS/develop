using CromosList.Data;
using CromosList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosAlbumesController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public UsuariosAlbumesController(CromosListDbContext context)
    {
        _context = context;
    }

    // Colecciones (álbumes) a los que el usuario está apuntado, con su progreso.
    [HttpGet("{usuarioId:long}")]
    public async Task<IActionResult> GetByUsuario(long usuarioId)
    {
        if (!await _context.Usuarios.AnyAsync(u => u.Id == usuarioId))
            return NotFound("El usuario no existe.");

        var albumes = await _context.UsuariosAlbumes
            .AsNoTracking()
            .Include(ua => ua.Album)
                .ThenInclude(a => a.Editorial)
            .Where(ua => ua.UsuarioId == usuarioId)
            .Select(ua => new
            {
                ua.AlbumId,
                ua.Album.Nombre,
                ua.Album.Temporada,
                Editorial = new { ua.Album.Editorial.Id, ua.Album.Editorial.Nombre }
            })
            .OrderBy(x => x.Nombre)
            .ToListAsync();

        var ids = albumes.Select(x => x.AlbumId).ToList();

        var totales = await _context.Cromos
            .AsNoTracking()
            .Where(c => ids.Contains(c.AlbumId))
            .GroupBy(c => c.AlbumId)
            .Select(g => new { AlbumId = g.Key, Total = g.Count() })
            .ToDictionaryAsync(g => g.AlbumId, g => g.Total);

        var tenidos = await _context.UsuariosCromo
            .AsNoTracking()
            .Where(uc => uc.UsuarioId == usuarioId && ids.Contains(uc.Cromo.AlbumId))
            .GroupBy(uc => uc.Cromo.AlbumId)
            .Select(g => new { AlbumId = g.Key, Total = g.Select(x => x.CromoId).Distinct().Count() })
            .ToDictionaryAsync(g => g.AlbumId, g => g.Total);

        var resultado = albumes.Select(x =>
        {
            var total = totales.GetValueOrDefault(x.AlbumId);
            var tiene = tenidos.GetValueOrDefault(x.AlbumId);
            return new
            {
                x.AlbumId,
                x.Nombre,
                x.Temporada,
                x.Editorial,
                TotalCromos = total,
                Tenidos = tiene,
                Faltan = total - tiene,
                Completado = total > 0 && tiene >= total
            };
        });

        return Ok(resultado);
    }

    // Detalle de progreso de una colección concreta de un usuario.
    [HttpGet("{usuarioId:long}/albumes/{albumId:long}")]
    public async Task<IActionResult> GetProgresoAlbum(long usuarioId, long albumId)
    {
        var album = await _context.Albums
            .AsNoTracking()
            .Where(a => a.Id == albumId)
            .Select(a => new { a.Id, a.Nombre, a.Temporada })
            .FirstOrDefaultAsync();

        if (album is null)
            return NotFound("La colección no existe.");

        var suscrito = await _context.UsuariosAlbumes
            .AnyAsync(ua => ua.UsuarioId == usuarioId && ua.AlbumId == albumId);

        var totalCromos = await _context.Cromos
            .CountAsync(c => c.AlbumId == albumId);

        var cromosTenidos = await _context.UsuariosCromo
            .AsNoTracking()
            .Where(uc => uc.UsuarioId == usuarioId && uc.Cromo.AlbumId == albumId)
            .Select(uc => uc.CromoId)
            .ToListAsync();

        return Ok(new
        {
            album.Id,
            album.Nombre,
            album.Temporada,
            Suscrito = suscrito,
            TotalCromos = totalCromos,
            Tenidos = cromosTenidos.Count,
            Faltan = totalCromos - cromosTenidos.Count,
            CromosTenidos = cromosTenidos
        });
    }

    // El usuario se apunta a una colección (se le asigna su copia).
    [HttpPost]
    public async Task<IActionResult> Suscribir([FromBody] SuscribirAlbumDto dto)
    {
        if (!await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId))
            return BadRequest("El usuario no existe.");

        if (!await _context.Albums.AnyAsync(a => a.Id == dto.AlbumId))
            return BadRequest("La colección no existe.");

        var yaExiste = await _context.UsuariosAlbumes
            .AnyAsync(ua => ua.UsuarioId == dto.UsuarioId && ua.AlbumId == dto.AlbumId);

        if (yaExiste)
            return Conflict("El usuario ya está apuntado a esta colección.");

        var usuarioAlbum = new UsuarioAlbum
        {
            UsuarioId = dto.UsuarioId,
            AlbumId = dto.AlbumId
        };

        _context.UsuariosAlbumes.Add(usuarioAlbum);
        await _context.SaveChangesAsync();

        return Ok(usuarioAlbum);
    }

    // El usuario se desapunta de una colección.
    [HttpDelete("{usuarioId:long}/{albumId:long}")]
    public async Task<IActionResult> Desuscribir(long usuarioId, long albumId)
    {
        var usuarioAlbum = await _context.UsuariosAlbumes
            .FirstOrDefaultAsync(ua => ua.UsuarioId == usuarioId && ua.AlbumId == albumId);

        if (usuarioAlbum is null)
            return NotFound("El usuario no está apuntado a esta colección.");

        _context.UsuariosAlbumes.Remove(usuarioAlbum);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class SuscribirAlbumDto
{
    public long UsuarioId { get; set; }
    public long AlbumId { get; set; }
}