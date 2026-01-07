using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class AvaliacaoJogoController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public AvaliacaoJogoController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> VotarLikeDislike([FromBody] AvaliacaoInput input)
        {
            var user = await _userManager.GetUserAsync(User);
            var avaliacao = await _context.AvaliacoesJogos
                .FirstOrDefaultAsync(a => a.UsuarioId == user.Id && a.JogoId == input.JogoId);

            if (avaliacao == null)
            {
                avaliacao = new AvaliacaoJogo
                {
                    UsuarioId = user.Id,
                    JogoId = input.JogoId,
                    Gostou = input.Gostou
                };
                _context.AvaliacoesJogos.Add(avaliacao);
            }
            else
            {
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
                .ToListAsync();

            return View(avaliacoes); // precisa de uma View Avaliacoes.cshtml
        }

        public class AvaliacaoInput
        {
            public int JogoId { get; set; }
            public bool Gostou { get; set; }
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



    }
}
