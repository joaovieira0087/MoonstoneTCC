using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Services; // Certifique-se de que o IGamificacaoService está aqui

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class AvaliacaoJogoController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IGamificacaoService _gamificacao;

        public AvaliacaoJogoController(
            UserManager<IdentityUser> userManager,
            AppDbContext context,
            IGamificacaoService gamificacao)
        {
            _userManager = userManager;
            _context = context;
            _gamificacao = gamificacao;
        }

        // Dashboard de Recompensas (O Painel com Gráficos)
        [HttpGet]
        public async Task<IActionResult> PainelRecompensas()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var progresso = await _gamificacao.ObterProgressoAsync(user.Id);
            return View(progresso); // Retorna a View com os dados de XP e porcentagem
        }

        [HttpPost]
        public async Task<IActionResult> VotarLikeDislike([FromBody] AvaliacaoInput input)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var avaliacao = await _context.AvaliacoesJogos
                .FirstOrDefaultAsync(a => a.UsuarioId == user.Id && a.JogoId == input.JogoId);

            if (avaliacao == null)
            {
                // PRIMEIRA AVALIAÇÃO: Cria o registro e concede XP
                avaliacao = new AvaliacaoJogo
                {
                    UsuarioId = user.Id,
                    JogoId = input.JogoId,
                    Gostou = input.Gostou
                };
                _context.AvaliacoesJogos.Add(avaliacao);

                // Lógica de Recompensa: Adiciona XP e verifica se ganha saldo
                await _gamificacao.AdicionarXPPorAvaliacaoAsync(user.Id);
            }
            else
            {
                // APENAS ATUALIZAÇÃO: Altera a opinião mas não dá XP novamente
                avaliacao.Gostou = input.Gostou;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> EstadoLikeDislike(int jogoId)
        {
            var user = await _userManager.GetUserAsync(User);
            var avaliacao = await _context.AvaliacoesJogos
                .FirstOrDefaultAsync(a => a.UsuarioId == user.Id && a.JogoId == jogoId);

            return Json(new { gostou = avaliacao?.Gostou });
        }

        [HttpGet]
        public async Task<IActionResult> Avaliacoes()
        {
            var user = await _userManager.GetUserAsync(User);
            var avaliacoes = await _context.AvaliacoesJogos
                .Include(a => a.Jogo)
                .Where(a => a.UsuarioId == user.Id)
                .OrderByDescending(a => a.Id)
                .ToListAsync();

            return View(avaliacoes);
        }

        [HttpPost]
        public async Task<IActionResult> RemoverAvaliacaoView(int jogoId)
        {
            var user = await _userManager.GetUserAsync(User);
            var avaliacao = await _context.AvaliacoesJogos
                .FirstOrDefaultAsync(a => a.UsuarioId == user.Id && a.JogoId == jogoId);

            if (avaliacao != null)
            {
                _context.AvaliacoesJogos.Remove(avaliacao);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Avaliacoes");
        }

        [HttpPost]
        public async Task<IActionResult> Remover([FromBody] AvaliacaoInput input)
        {
            var user = await _userManager.GetUserAsync(User);
            var avaliacao = await _context.AvaliacoesJogos
                .FirstOrDefaultAsync(a => a.UsuarioId == user.Id && a.JogoId == input.JogoId);

            if (avaliacao != null)
            {
                _context.AvaliacoesJogos.Remove(avaliacao);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        public class AvaliacaoInput
        {
            public int JogoId { get; set; }
            public bool Gostou { get; set; }
        }
    }
}