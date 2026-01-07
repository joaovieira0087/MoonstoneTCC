using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Services;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class CancelamentoPedidoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICarteiraService _carteiraService;

        public CancelamentoPedidoController(
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            ICarteiraService carteiraService)
        {
            _context = context;
            _userManager = userManager;
            _carteiraService = carteiraService;
        }

        [HttpGet]
        public async Task<IActionResult> Criar(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens)
                    .ThenInclude(i => i.Jogo)
                .FirstOrDefaultAsync(p => p.PedidoId == id && p.UserId == user.Id);

            if (pedido == null)
                return NotFound();

            if (pedido.StatusPedido == "Cancelado")
            {
                TempData["MensagemErro"] = "Este pedido já foi cancelado.";
                return RedirectToAction("Index", "MeusPedidos");
            }

            return View(pedido); // View: Views/CancelamentoPedido/Criar.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(int id, string? motivo)
        {
            var user = await _userManager.GetUserAsync(User);

            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens)
                .FirstOrDefaultAsync(p => p.PedidoId == id && p.UserId == user.Id);

            if (pedido == null)
                return NotFound();

            if (pedido.StatusPedido == "Cancelado")
            {
                TempData["MensagemErro"] = "Este pedido já foi cancelado.";
                return RedirectToAction("Index", "MeusPedidos");
            }

            // Atualiza o status e data
            pedido.StatusPedido = "Cancelado";
            pedido.DataCancelamento = DateTime.Now;

            // Repor estoque dos jogos do pedido
            foreach (var item in pedido.PedidoItens)
            {
                var jogo = await _context.Jogos.FindAsync(item.JogoId);
                if (jogo != null)
                {
                    jogo.QuantidadeEstoque += item.Quantidade;
                }
            }

            var cancelamento = new CancelamentoPedido
            {
                PedidoId = id,
                UsuarioId = user.Id,
                Motivo = motivo,
                DataCancelamento = DateTime.Now
            };

            _context.CancelamentosPedidos.Add(cancelamento);
            await _context.SaveChangesAsync();

            // >>> Integração com CARTEIRA: crédito de estorno (sempre volta como saldo)
            // Evita estornar duas vezes o mesmo pedido.
            if (!await _carteiraService.JaEstornadoAsync(pedido.PedidoId))
            {
                await _carteiraService.EstornarPedidoAsync(user.Id, pedido.PedidoTotal, pedido.PedidoId);
            }

            TempData["MensagemSucesso"] = "Seu pedido foi cancelado com sucesso, o estoque foi atualizado e o valor foi creditado na sua carteira.";
            return RedirectToAction("Index", "MeusPedidos");
        }
    }
}
