using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class EstoqueController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EstoqueController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarAviso(int jogoId)
        {
            var user = await _userManager.GetUserAsync(User);

            bool jaExiste = await _context.AvisosEstoque
                .AnyAsync(a => a.UsuarioId == user.Id && a.JogoId == jogoId && !a.Avisado); // <-- corrigido

            if (!jaExiste)
            {
                var aviso = new AvisoEstoque
                {
                    UsuarioId = user.Id, // <-- corrigido
                    JogoId = jogoId,
                    Avisado = false
                };

                _context.AvisosEstoque.Add(aviso);
                await _context.SaveChangesAsync();
            }

            TempData["MensagemSucesso"] = "Você será avisado quando este jogo estiver disponível.";
            return RedirectToAction("Details", "Jogo", new { jogoId });
        }


    }
}
