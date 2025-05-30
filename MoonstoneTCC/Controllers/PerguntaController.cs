using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.EntityFrameworkCore;


[Authorize]
public class PerguntaController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public PerguntaController(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Criar()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(PerguntaUsuario pergunta)
    {
        var user = await _userManager.GetUserAsync(User);
        pergunta.UsuarioId = user.Id;
        pergunta.DataEnvio = DateTime.Now;

        _context.PerguntasUsuarios.Add(pergunta);
        await _context.SaveChangesAsync();

        return RedirectToAction("MinhasPerguntas");
    }

    public async Task<IActionResult> MinhasPerguntas()
    {
        var user = await _userManager.GetUserAsync(User);
        var perguntas = await _context.PerguntasUsuarios
            .Where(p => p.UsuarioId == user.Id)
            .OrderByDescending(p => p.DataEnvio)
            .ToListAsync();

        return View(perguntas);
    }





}
