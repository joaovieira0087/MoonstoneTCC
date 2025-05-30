using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Context;
using Microsoft.EntityFrameworkCore;


[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminPerguntasController : Controller
{
    private readonly AppDbContext _context;

    public AdminPerguntasController(AppDbContext context)
    {
        _context = context;
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

        return RedirectToAction("Index");
    }
}
