using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MoonstoneTCC.Context;
using System.Linq;
using System.Threading.Tasks;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminPromocoesController : Controller
    {
        private readonly AppDbContext _context;

        public AdminPromocoesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var jogos = _context.Jogos.ToList();
            return View(jogos);
        }

        [HttpPost]
        public async Task<IActionResult> AplicarPromocao(int jogoId, int porcentagem)
        {
            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo != null && porcentagem > 0)
            {
                var desconto = (jogo.Preco * porcentagem) / 100;
                jogo.PrecoPromocional = jogo.Preco - desconto;
                jogo.PorcentagemDesconto = porcentagem;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoverPromocao(int jogoId)
        {
            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo != null)
            {
                jogo.PrecoPromocional = null;
                jogo.PorcentagemDesconto = null;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
