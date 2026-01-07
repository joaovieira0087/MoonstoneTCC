using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.ViewModels;
using MoonstoneTCC.Models;


[Authorize]
public class DesenvolvedoraController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public DesenvolvedoraController(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Perfil(int id)
    {
        var desenvolvedora = await _context.Desenvolvedoras
            .Include(d => d.Jogos)
            .Include(d => d.Seguidores)
            .FirstOrDefaultAsync(d => d.DesenvolvedoraId == id);

        if (desenvolvedora == null) return NotFound();

        var userId = User.Identity.IsAuthenticated ? _userManager.GetUserId(User) : null;
        var estaSeguindo = userId != null && await _context.SeguidoresDesenvolvedoras
            .AnyAsync(s => s.UsuarioId == userId && s.DesenvolvedoraId == id);

        var totalSeguidores = await _context.SeguidoresDesenvolvedoras
            .CountAsync(s => s.DesenvolvedoraId == id);

        var viewModel = new DesenvolvedoraPerfilViewModel
        {
            Desenvolvedora = desenvolvedora,
            EstaSeguindo = estaSeguindo,
            TotalSeguidores = totalSeguidores,
            TotalSeguindo = 0 
        };

        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> ToggleSeguir(int desenvolvedoraId)
    {
        var user = await _userManager.GetUserAsync(User);
        var existente = await _context.SeguidoresDesenvolvedoras
            .FirstOrDefaultAsync(s => s.UsuarioId == user.Id && s.DesenvolvedoraId == desenvolvedoraId);

        if (existente != null)
        {
            _context.SeguidoresDesenvolvedoras.Remove(existente);
        }
        else
        {
            _context.SeguidoresDesenvolvedoras.Add(new SeguidorDesenvolvedora
            {
                UsuarioId = user.Id,
                DesenvolvedoraId = desenvolvedoraId
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Perfil", new { id = desenvolvedoraId });
    }

    [AllowAnonymous]
    [HttpGet("/Desenvolvedora/Conexoes/{id}")]
    public async Task<IActionResult> Conexoes(int id)
    {
        var desenvolvedora = await _context.Desenvolvedoras
            .Include(d => d.Seguidores)
            .ThenInclude(s => s.Usuario)
            .FirstOrDefaultAsync(d => d.DesenvolvedoraId == id);

        if (desenvolvedora == null)
            return NotFound();

        var seguidores = desenvolvedora.Seguidores;

        ViewBag.Desenvolvedora = desenvolvedora;
        ViewBag.Seguidores = seguidores;

        return View("ConexoesDesenvolvedora"); // crie essa view se quiser
    }

}
