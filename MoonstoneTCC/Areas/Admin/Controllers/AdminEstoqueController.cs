// Controller: Areas/Admin/Controllers/AdminEstoqueController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MoonstoneTCC.Hubs; // ou o namespace onde seu hub está


namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminEstoqueController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificacaoHub> _hubContext;

        public AdminEstoqueController(AppDbContext context, IHubContext<NotificacaoHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var jogos = await _context.Jogos
                .Include(j => j.Categoria)
                .OrderBy(j => j.Nome)
                .ToListAsync();
            return View(jogos);
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarEstoque(int jogoId, int novaQuantidade)
        {
            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo != null)
            {
                bool estavaZerado = jogo.QuantidadeEstoque == 0;

                jogo.QuantidadeEstoque = novaQuantidade;

                if (estavaZerado && novaQuantidade > 0)
                {
                    await VerificarNotificacoes(jogo);
                }

                await _context.SaveChangesAsync();
                TempData[$"EstoqueAtualizado_{jogoId}"] = true;
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> IncrementarEstoque(int jogoId)
        {
            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo != null)
            {
                jogo.QuantidadeEstoque++;

                // Notifica usuários se voltou ao estoque
                await VerificarNotificacoes(jogo);

                await _context.SaveChangesAsync();
                TempData[$"EstoqueAtualizado_{jogoId}"] = true;
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> UsuariosAguardando(int jogoId)
        {
            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo == null) return NotFound();

            var avisos = await _context.AvisosEstoque
                .Include(a => a.Usuario)
                .Where(a => a.JogoId == jogoId && !a.Avisado)
                .ToListAsync();

            ViewBag.JogoNome = jogo.Nome;
            return View("UsuariosAguardando", avisos);
        }

        private async Task VerificarNotificacoes(Jogo jogo)
        {
            var interessados = await _context.AvisosEstoque
                .Where(a => a.JogoId == jogo.JogoId && !a.Avisado)
                .ToListAsync();

            foreach (var aviso in interessados)
            {
                // 1. Envia notificação via SignalR
                await _hubContext.Clients.User(aviso.UsuarioId)
                    .SendAsync("ReceberNotificacao", $"O jogo \"{jogo.Nome}\" voltou ao estoque!");

                // 2. Salva notificação no histórico
                _context.NotificacoesEstoque.Add(new NotificacaoEstoque
                {
                    UsuarioId = aviso.UsuarioId,
                    JogoId = jogo.JogoId,
                    Mensagem = $"O jogo \"{jogo.Nome}\" está de volta ao estoque!"
                });

                // 3. Marca como avisado
                aviso.Avisado = true;
            }
        }


        [HttpPost]
        public async Task<IActionResult> AdicionarEstoque(int jogoId, int quantidade)
        {
            var jogo = await _context.Jogos.FindAsync(jogoId);

            if (jogo == null)
                return NotFound();

            jogo.QuantidadeEstoque += quantidade;

            // Se voltou ao estoque, avisa quem estava esperando
            if (jogo.QuantidadeEstoque > 0)
            {
                var interessados = await _context.AvisosEstoque
                    .Where(a => a.JogoId == jogo.JogoId && !a.Avisado)
                    .ToListAsync();

                foreach (var aviso in interessados)
                {
                    // 1. Avisa via SignalR individualmente
                    await _hubContext.Clients.User(aviso.UsuarioId)
                        .SendAsync("ReceberNotificacao", $"O jogo \"{jogo.Nome}\" voltou ao estoque!");

                    // 2. Salva o histórico da notificação
                    _context.NotificacoesEstoque.Add(new NotificacaoEstoque
                    {
                        UsuarioId = aviso.UsuarioId,
                        JogoId = jogo.JogoId,
                        Mensagem = $"O jogo \"{jogo.Nome}\" está de volta ao estoque!"
                    });

                    // 3. Marca como avisado para não repetir
                    aviso.Avisado = true;
                }
            }

                await _context.SaveChangesAsync();

                TempData["MensagemSucesso"] = "Estoque atualizado com sucesso!";
                return RedirectToAction("Index");
        }
    }
}