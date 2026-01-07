using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using System.Threading.Tasks;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class NotificacoesEstoqueController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificacoesEstoqueController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var notificacoes = await _context.NotificacoesEstoque
                .Include(n => n.Jogo)
                .Where(n => n.UsuarioId == user.Id && !n.Lida)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();

            return View(notificacoes);
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLida(int id)
        {
            var notificacao = await _context.NotificacoesEstoque.FindAsync(id);

            if (notificacao != null)
            {
                notificacao.Lida = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
