using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class ComunicadoUsuarioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ComunicadoUsuarioController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Comunicados()
        {
            var comunicados = await _context.Comunicados
                .OrderByDescending(c => c.DataCriacao)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            ViewBag.UsuarioId = user?.Id;

            return View(comunicados);
        }

        [HttpPost]
        public async Task<IActionResult> Votar(int ComunicadoId, string OpcaoEscolhida)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null && !string.IsNullOrEmpty(OpcaoEscolhida))
            {
                var resposta = new RespostaUsuario
                {
                    ComunicadoId = ComunicadoId,
                    UsuarioId = user.Id,
                    OpcaoEscolhida = OpcaoEscolhida,
                    DataResposta = DateTime.Now
                };

                _context.RespostasUsuarios.Add(resposta);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Comunicados");
        }

        [HttpPost]
        public async Task<IActionResult> ResponderPergunta(int ComunicadoId, string TextoResposta)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null && !string.IsNullOrEmpty(TextoResposta))
            {
                var resposta = new RespostaUsuario
                {
                    ComunicadoId = ComunicadoId,
                    UsuarioId = user.Id,
                    TextoResposta = TextoResposta,
                    DataResposta = DateTime.Now
                };

                _context.RespostasUsuarios.Add(resposta);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Comunicados");
        }


        [HttpPost]
        public async Task<IActionResult> ResponderPerguntaMultipla(int ComunicadoId, int PerguntaId, string TextoResposta)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null && !string.IsNullOrEmpty(TextoResposta))
            {
                var resposta = new RespostaUsuario
                {
                    ComunicadoId = ComunicadoId,
                    UsuarioId = user.Id,
                    TextoResposta = $"[{PerguntaId}] {TextoResposta}", // marca qual pergunta foi respondida
                    DataResposta = DateTime.Now
                };

                _context.RespostasUsuarios.Add(resposta);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Comunicados");
        }


        [HttpPost]
        public async Task<IActionResult> ResponderVariasPerguntas(RespostaMultiplaViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                foreach (var resposta in model.Respostas)
                {
                    if (!string.IsNullOrWhiteSpace(resposta.Texto))
                    {
                        var nova = new RespostaUsuario
                        {
                            ComunicadoId = model.ComunicadoId,
                            UsuarioId = user.Id,
                            TextoResposta = $"[{resposta.PerguntaId}] {resposta.Texto}",
                            DataResposta = DateTime.Now
                        };

                        _context.RespostasUsuarios.Add(nova);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Comunicados");
        }




    }
}
