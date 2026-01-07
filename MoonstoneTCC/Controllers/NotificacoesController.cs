using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class NotificacoesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificacoesController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ Método que você passou
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var notificacoesEstoque = await _context.NotificacoesEstoque
                .Include(n => n.Jogo)
                .Where(n => n.UsuarioId == user.Id)
                .ToListAsync();

            var notificacoesPedido = await _context.NotificacoesPedido
                .Include(n => n.Pedido)
                .Where(n => n.UsuarioId == user.Id)
                .ToListAsync();

            var viewModel = new NotificacoesViewModel
            {
                Estoque = notificacoesEstoque,
                Pedido = notificacoesPedido
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLidaEstoque(int id)
        {
            var noti = await _context.NotificacoesEstoque.FindAsync(id);
            if (noti == null) return NotFound();

            noti.Lida = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLidaPedido(int id)
        {
            var noti = await _context.NotificacoesPedido.FindAsync(id);
            if (noti == null) return NotFound();

            noti.Lida = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
