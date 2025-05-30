using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminComunicadoController : Controller
    {
        private readonly AppDbContext _context;

        public AdminComunicadoController(AppDbContext context)
        {
            _context = context;
        }

        // LISTAR
        public async Task<IActionResult> Index()
        {
            var comunicados = await _context.Comunicados.OrderByDescending(c => c.DataCriacao).ToListAsync();
            return View(comunicados);
        }

        // CRIAR
        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Comunicado comunicado, List<string> Perguntas)
        {
            if (ModelState.IsValid)
            {
                _context.Comunicados.Add(comunicado);
                await _context.SaveChangesAsync();

                if (comunicado.Tipo == TipoComunicado.Pergunta && Perguntas != null)
                {
                    foreach (var texto in Perguntas.Where(p => !string.IsNullOrWhiteSpace(p)))
                    {
                        var pergunta = new PerguntaComunicado
                        {
                            ComunicadoId = comunicado.Id,
                            TextoPergunta = texto
                        };
                        _context.PerguntasComunicados.Add(pergunta);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            return View(comunicado);
        }

        // EDITAR
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var comunicado = await _context.Comunicados.FindAsync(id);
            if (comunicado == null)
                return NotFound();

            return View(comunicado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Comunicado comunicado)
        {
            if (ModelState.IsValid)
            {
                _context.Comunicados.Update(comunicado);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(comunicado);
        }

        // EXCLUIR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            var comunicado = await _context.Comunicados.FindAsync(id);
            if (comunicado == null)
                return NotFound();

            // Excluir perguntas relacionadas, se existirem
            var perguntas = _context.PerguntasComunicados.Where(p => p.ComunicadoId == id);
            _context.PerguntasComunicados.RemoveRange(perguntas);

            // Excluir respostas relacionadas
            var respostas = _context.RespostasUsuarios.Where(r => r.ComunicadoId == id);
            _context.RespostasUsuarios.RemoveRange(respostas);

            _context.Comunicados.Remove(comunicado);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // VER RESPOSTAS (simples)
        [HttpGet]
        public async Task<IActionResult> Respostas(int id)
        {
            var respostas = await _context.RespostasUsuarios
                .Include(r => r.Usuario)
                .Include(r => r.Comunicado)
                .Where(r => r.ComunicadoId == id)
                .OrderByDescending(r => r.DataResposta)
                .ToListAsync();

            return View(respostas);
        }
    }
}
