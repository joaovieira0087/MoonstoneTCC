using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using System.Security.Claims;

namespace MoonstoneTCC.Components
{
    public class NotificacaoResumoViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificacaoResumoViewComponent(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return Content(string.Empty);

            var user = await _userManager.GetUserAsync((ClaimsPrincipal)User);

            var notificacoesEstoqueNaoLidas = await _context.NotificacoesEstoque
                .Where(n => n.UsuarioId == user.Id && !n.Lida)
                .CountAsync();

            var notificacoesPedidoNaoLidas = await _context.NotificacoesPedido
                .Where(n => n.UsuarioId == user.Id && !n.Lida)
                .CountAsync();

            var totalNaoLidas = notificacoesEstoqueNaoLidas + notificacoesPedidoNaoLidas;

            return View(totalNaoLidas); // Vai para Views/Shared/Components/NotificacaoResumo/Default.cshtml
        }
    }
}
