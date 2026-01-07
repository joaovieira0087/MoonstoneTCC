using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Hubs;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminStatusPedidosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificacaoHub> _hubContext;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminStatusPedidosController(AppDbContext context, IHubContext<NotificacaoHub> hubContext, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .OrderByDescending(p => p.PedidoEnviado)
                .ToListAsync();

            return View(pedidos);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();

            ViewBag.StatusList = new SelectList(new[]
            {
                "Pagamento Confirmado",
                "Preparando Pedido",
                "Pedido Enviado",
                "Pedido Entregue",
                "Cancelado"
            }, pedido.StatusPedido);

            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PedidoId,StatusPedido")] Pedido pedido)
        {
            if (id != pedido.PedidoId)
                return NotFound();

            var pedidoExistente = await _context.Pedidos.FindAsync(id);
            if (pedidoExistente == null)
                return NotFound();

            // Verifica se houve alteração no status
            if (pedidoExistente.StatusPedido != pedido.StatusPedido)
            {
                pedidoExistente.StatusPedido = pedido.StatusPedido;

                // Cria e salva a notificação no banco
                _context.NotificacoesPedido.Add(new NotificacaoPedido
                {
                    UsuarioId = pedidoExistente.UserId,
                    PedidoId = pedidoExistente.PedidoId,
                    Mensagem = $"O status do seu pedido #{pedidoExistente.PedidoId} foi alterado para \"{pedido.StatusPedido}\".",
                    DataCriacao = DateTime.Now,
                    Lida = false
                });

                // Envia via SignalR para o usuário
                await _hubContext.Clients.User(pedidoExistente.UserId)
                    .SendAsync("ReceberNotificacao", $"Seu pedido #{pedidoExistente.PedidoId} agora está como \"{pedido.StatusPedido}\"");
            }

            _context.Update(pedidoExistente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
