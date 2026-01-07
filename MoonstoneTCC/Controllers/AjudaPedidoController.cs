using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class AjudaPedidoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AjudaPedidoController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Exibe o formulário de ajuda vinculado a um pedido
        [HttpGet]
        public async Task<IActionResult> Criar(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens)
                .ThenInclude(pi => pi.Jogo)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null)
                return NotFound();

            return View(pedido); // Views/AjudaPedido/Criar.cshtml
        }

        // Processa a solicitação de ajuda
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(int id, string tipoProblema, string? descricao)
        {
            var user = await _userManager.GetUserAsync(User);

            var ajuda = new AjudaPedido
            {
                PedidoId = id,
                UsuarioId = user.Id,
                TipoProblema = tipoProblema,
                Descricao = descricao,
                DataEnvio = DateTime.Now
            };

            _context.AjudasPedidos.Add(ajuda);
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Sua solicitação foi enviada com sucesso!";
            return RedirectToAction("MinhasSolicitacoes");
        }

        // Lista todas as solicitações de ajuda feitas pelo usuário logado
        public async Task<IActionResult> MinhasSolicitacoes()
        {
            var user = await _userManager.GetUserAsync(User);

            var lista = await _context.AjudasPedidos
                .Include(a => a.Pedido)
                .Where(a => a.UsuarioId == user.Id)
                .OrderByDescending(a => a.DataEnvio)
                .ToListAsync();

            return View(lista); // Views/AjudaPedido/MinhasSolicitacoes.cshtml
        }
    }
}
