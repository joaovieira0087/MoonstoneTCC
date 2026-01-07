using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.EntityFrameworkCore;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class PerguntaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PerguntaController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Lista todas as perguntas feitas pelo usuário logado
        public async Task<IActionResult> MinhasPerguntas()
        {
            var user = await _userManager.GetUserAsync(User);

            var perguntas = await _context.PerguntasUsuarios
                .Where(p => p.UsuarioId == user.Id)
                .OrderByDescending(p => p.DataEnvio)
                .ToListAsync();

            return View(perguntas); // Views/Pergunta/MinhasPerguntas.cshtml
        }

        // Exibe o formulário para criar uma nova pergunta
        [HttpGet]
        public async Task<IActionResult> Criar(int? pedidoId)
        {
            ViewBag.PedidoId = pedidoId;

            if (pedidoId.HasValue)
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.PedidoItens)
                        .ThenInclude(i => i.Jogo)
                    .FirstOrDefaultAsync(p => p.PedidoId == pedidoId.Value);

                ViewBag.Pedido = pedido;
            }

            return View(); // Views/Pergunta/Criar.cshtml
        }

        // Processa o envio da pergunta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(string Titulo, string Mensagem, int? PedidoId)
        {
            var user = await _userManager.GetUserAsync(User);

            var pergunta = new PerguntaUsuario
            {
                UsuarioId = user.Id,
                Titulo = Titulo,
                Mensagem = Mensagem,
                DataEnvio = DateTime.Now,
                PedidoId = PedidoId
            };

            _context.PerguntasUsuarios.Add(pergunta);
            await _context.SaveChangesAsync();

            return RedirectToAction("MinhasPerguntas", "Pergunta");
        }
    }
}
