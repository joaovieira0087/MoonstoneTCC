using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class HistoricoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HistoricoController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Historico/
        public async Task<IActionResult> Index(int? mes, int? ano)
        {
            var user = await _userManager.GetUserAsync(User);

            var query = _context.HistoricoVisualizacoes
                .Where(h => h.UsuarioId == user.Id);

            if (mes.HasValue)
                query = query.Where(h => h.DataVisualizacao.Month == mes.Value);

            if (ano.HasValue)
                query = query.Where(h => h.DataVisualizacao.Year == ano.Value);

            query = query.Include(h => h.Jogo);

            var historico = await query
                .OrderByDescending(h => h.DataVisualizacao)
                .ToListAsync();

            var agrupado = historico
                .GroupBy(h => new { h.DataVisualizacao.Year, h.DataVisualizacao.Month })
                .OrderByDescending(g => g.Key.Year)
                .ThenByDescending(g => g.Key.Month)
                .ToList();

            ViewBag.HistoricoAgrupado = agrupado;

            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar([FromBody] HistoricoRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || request == null || request.JogoId <= 0)
                return BadRequest();

            var jaExiste = await _context.HistoricoVisualizacoes
                .AnyAsync(h => h.UsuarioId == user.Id && h.JogoId == request.JogoId);

            if (!jaExiste)
            {
                var historico = new HistoricoVisualizacao
                {
                    UsuarioId = user.Id,
                    JogoId = request.JogoId,
                    DataVisualizacao = DateTime.Now
                };

                _context.HistoricoVisualizacoes.Add(historico);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        public class HistoricoRequest
        {
            public int JogoId { get; set; }
        }


        // (Opcional) Deletar histórico inteiro
        [HttpPost]
        public async Task<IActionResult> Limpar()
        {
            var user = await _userManager.GetUserAsync(User);

            var historico = _context.HistoricoVisualizacoes.Where(h => h.UsuarioId == user.Id);
            _context.HistoricoVisualizacoes.RemoveRange(historico);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
