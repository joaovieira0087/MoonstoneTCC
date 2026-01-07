using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.EntityFrameworkCore;


[Authorize]
public class SeguidorController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public SeguidorController(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SeguirOuDeixarDeSeguir(string id)
    {
        var usuarioAtual = await _userManager.GetUserAsync(User);
        var usuarioAtualId = usuarioAtual.Id;

        if (id == usuarioAtualId)
        {
            return BadRequest("Você não pode seguir a si mesmo.");
        }

        var seguidor = await _context.SeguidoresUsuarios
            .FirstOrDefaultAsync(s => s.SeguidorId == usuarioAtualId && s.SeguidoId == id);

        if (seguidor != null)
        {
            _context.SeguidoresUsuarios.Remove(seguidor);
        }
        else
        {
            _context.SeguidoresUsuarios.Add(new SeguidorUsuario
            {
                SeguidorId = usuarioAtualId,
                SeguidoId = id,
                Data = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("PerfilPublico", "Usuario", new { id });
    }


    [HttpPost]
    public async Task<IActionResult> ToggleSeguir(string seguidoId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user.Id == seguidoId) return BadRequest();

        var existente = await _context.SeguidoresUsuarios
            .FirstOrDefaultAsync(s => s.SeguidorId == user.Id && s.SeguidoId == seguidoId);

        if (existente != null)
        {
            _context.SeguidoresUsuarios.Remove(existente);
        }
        else
        {
            _context.SeguidoresUsuarios.Add(new SeguidorUsuario
            {
                SeguidorId = user.Id,
                SeguidoId = seguidoId
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("PerfilPublico", "Perfil", new { id = seguidoId });
    }

    public async Task<IActionResult> MinhasConexoes()
    {
        var user = await _userManager.GetUserAsync(User);

        var seguindo = await _context.SeguidoresUsuarios
            .Where(s => s.SeguidorId == user.Id)
            .Include(s => s.Seguido)
            .ToListAsync();

        var seguidores = await _context.SeguidoresUsuarios
            .Where(s => s.SeguidoId == user.Id)
            .Include(s => s.Seguidor)
            .ToListAsync();

        ViewBag.Seguindo = seguindo;
        ViewBag.Seguidores = seguidores;

        return View();
    }

    [AllowAnonymous]
    [HttpGet("/Seguidor/Conexoes/{id}")]
    public async Task<IActionResult> Conexoes(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
            return NotFound();

        var seguindo = await _context.SeguidoresUsuarios
            .Where(s => s.SeguidorId == id)
            .Include(s => s.Seguido)
            .ToListAsync();

        var seguidores = await _context.SeguidoresUsuarios
            .Where(s => s.SeguidoId == id)
            .Include(s => s.Seguidor)
            .ToListAsync();

        ViewBag.UsuarioPerfil = usuario;
        ViewBag.Seguindo = seguindo;
        ViewBag.Seguidores = seguidores;

        return View("Conexoes");
    }

}

