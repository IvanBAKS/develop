using CromosList.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CromosList.Dtos;

namespace CromosList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosCromosController : ControllerBase
{
    private readonly CromosListDbContext _context;

    public UsuariosCromosController(CromosListDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var usuariosCromos = await _context.UsuariosCromo
            .AsNoTracking()
            .ToListAsync();

        return Ok(usuariosCromos);
    }

    [HttpGet("{usuarioId:long}")]
    public async Task<IActionResult> GetByUsuario(long usuarioId)
    {
        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == usuarioId);

        if (!usuarioExiste)
            return NotFound("El usuario no existe.");

        var cromos = await _context.UsuariosCromo
            .AsNoTracking()
            .Where(uc => uc.UsuarioId == usuarioId)
            .Include(uc => uc.Cromo)
                .ThenInclude(c => c.Jugador)
            .Include(uc => uc.Cromo)
                .ThenInclude(c => c.Equipo)
            .Include(uc => uc.Cromo)
                .ThenInclude(c => c.Edicion)
            .Include(uc => uc.Cromo)
                .ThenInclude(c => c.TipoCromo)
            .Select(uc => new
            {
                uc.Cromo.Id,
                uc.Cromo.Numero,

                Jugador = new
                {
                    uc.Cromo.Jugador.Id,
                    uc.Cromo.Jugador.Nombre
                },

                Equipo = new
                {
                    uc.Cromo.Equipo.Id,
                    uc.Cromo.Equipo.Nombre
                },

                Edicion = new
                {
                    uc.Cromo.Edicion.Id,
                    uc.Cromo.Edicion.Nombre
                },

                TipoCromo = new
                {
                    uc.Cromo.TipoCromo.Id,
                    uc.Cromo.TipoCromo.Nombre
                }
            })
            .ToListAsync();

        return Ok(cromos);
    }

    [HttpPost]
    public async Task<IActionResult> AddCromo(CreateUsuarioCromoDto dto)
    {
        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == dto.UsuarioId);

        if (!usuarioExiste)
            return BadRequest("El usuario no existe.");

        var cromoExiste = await _context.Cromos
            .AnyAsync(c => c.Id == dto.CromoId);

        if (!cromoExiste)
            return BadRequest("El cromo no existe.");

        var yaExiste = await _context.UsuariosCromo
            .AnyAsync(uc =>
                uc.UsuarioId == dto.UsuarioId &&
                uc.CromoId == dto.CromoId);

        if (yaExiste)
            return Conflict("El usuario ya tiene este cromo.");

        var usuarioCromo = new Models.UsuarioCromo
        {
            UsuarioId = dto.UsuarioId,
            CromoId = dto.CromoId
        };

        _context.UsuariosCromo.Add(usuarioCromo);

        await _context.SaveChangesAsync();

        return Ok(usuarioCromo);
    }
}