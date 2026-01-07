using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Context;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Services;


[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminPerguntasController : Controller
{
    private readonly AppDbContext _context;
    private readonly LoggerAdminService _logger;


    public AdminPerguntasController(AppDbContext context, LoggerAdminService logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task<IActionResult> Index()
    {
        var perguntas = await _context.PerguntasUsuarios.Include(p => p.Usuario).ToListAsync();
        return View(perguntas);
    }

    [HttpGet]
    public async Task<IActionResult> Responder(int id)
    {
        var pergunta = await _context.PerguntasUsuarios.FindAsync(id);
        return View(pergunta);
    }

    [HttpPost]
    public async Task<IActionResult> Responder(int id, string resposta)
    {
        var pergunta = await _context.PerguntasUsuarios.FindAsync(id);
        if (pergunta == null) return NotFound();

        pergunta.RespostaAdmin = resposta;
        pergunta.DataResposta = DateTime.Now;
        pergunta.Respondido = true;

        _context.Update(pergunta);
        await _context.SaveChangesAsync();
        await _logger.RegistrarAcaoAsync($"Respondeu à pergunta de: {pergunta.Usuario?.UserName}");

        return RedirectToAction("Index");
    }
}
