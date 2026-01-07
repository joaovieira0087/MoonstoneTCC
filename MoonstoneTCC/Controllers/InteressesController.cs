using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.ViewModels;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class InteressesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public InteressesController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Selecionar()
        {
            var user = await _userManager.GetUserAsync(User);

            var interessesUsuario = await _context.InteressesUsuarios
                .Where(i => i.UsuarioId == user.Id)
                .Select(i => i.Interesse)
                .ToListAsync();

            var todos = new List<string>
            {
                "Aventura", "Ação", "Terror", "RPG", "FPS", "Mundo Aberto", "Estratégia", "Esportes",
                "Simulação", "Corrida", "Puzzle", "Multiplayer", "Singleplayer", "Hack and Slash",
                "Plataforma", "Stealth", "História Interativa", "Battle Royale", "MOBA", "Sandbox",
                "Sobrevivência", "Metroidvania", "Indie", "Futurista", "Cyberpunk", "Anime",
                "Realismo", "Guerra", "Zumbi", "Fantasioso", "Espacial", "Cooperativo", "Competitivo",
                "Casual", "Hardcore", "Pixel Art", "VR", "MMORPG", "Narrativo", "Sci-fi",
                "Musical", "Card Game", "Tática", "Horror Psicológico", "Crime", "Western",
                "8-bit", "Criativo", "Remake", "Remaster", "Visual Novel", "Clássico",
                "Free to Play", "Pay to Win", "Baseado em Filme", "Aventura Gráfica", "Treinamento Mental"
            };

            var model = new UserInteressesViewModel
            {
                InteressesSelecionados = interessesUsuario,
                TodosOsInteresses = todos
            };

            return View("Selecionar", model);
        }

        [HttpPost]
        public async Task<IActionResult> Salvar(List<string> InteressesSelecionados)
        {
            var user = await _userManager.GetUserAsync(User);

            var antigos = _context.InteressesUsuarios.Where(i => i.UsuarioId == user.Id);
            _context.InteressesUsuarios.RemoveRange(antigos);

            foreach (var interesse in InteressesSelecionados.Take(7))
            {
                _context.InteressesUsuarios.Add(new InteresseUsuario
                {
                    UsuarioId = user.Id,
                    Interesse = interesse
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Visualizar", "Perfil");

        }

    }
}
