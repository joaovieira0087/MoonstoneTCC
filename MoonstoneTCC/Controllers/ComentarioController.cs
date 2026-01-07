using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class ComentarioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ComentarioController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comentar(int jogoId, string texto, int avaliacao)
        {
            // Validação da nota
            if (avaliacao < 1 || avaliacao > 5)
            {
                TempData["Erro"] = "Avaliação inválida. Selecione entre 1 e 5 estrelas.";
                return RedirectToAction("Details", "Jogo", new { jogoId });
            }

            // Verifica se o usuário está autenticado
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Erro"] = "Usuário não autenticado.";
                return RedirectToAction("Login", "Account");
            }

            // Verifica se o jogo existe
            var jogoExiste = await _context.Jogos.AnyAsync(j => j.JogoId == jogoId);
            if (!jogoExiste)
            {
                TempData["Erro"] = "O jogo selecionado não foi encontrado.";
                return RedirectToAction("Index", "Jogo");
            }

            // Verifica se o usuário já comentou esse jogo
            var comentarioExistente = await _context.ComentariosJogo
                .AnyAsync(c => c.UsuarioId == user.Id && c.JogoId == jogoId);

            if (comentarioExistente)
            {
                TempData["Erro"] = "Você já avaliou este jogo.";
                return RedirectToAction("Details", "Jogo", new { jogoId });
            }

            // Verifica se o usuário comentou recentemente (últimos 5 minutos)
            var comentarioRecente = await _context.ComentariosJogo
                .Where(c => c.UsuarioId == user.Id)
                .OrderByDescending(c => c.Data)
                .FirstOrDefaultAsync();

            if (comentarioRecente != null && comentarioRecente.Data > DateTime.Now.AddMinutes(-5))
            {
                TempData["Erro"] = "Você pode comentar novamente após 5 minutos.";
                return RedirectToAction("Details", "Jogo", new { jogoId });
            }

            // Cria o novo comentário
            var novoComentario = new ComentarioJogo
            {
                JogoId = jogoId,
                UsuarioId = user.Id,
                Texto = texto?.Trim() ?? "",
                Avaliacao = avaliacao,
                Data = DateTime.Now
            };

            _context.ComentariosJogo.Add(novoComentario);
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Comentário enviado com sucesso!";
            return RedirectToAction("Details", "Jogo", new { jogoId });
        }




        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var comentario = await _context.ComentariosJogo.FindAsync(id);
            if (comentario == null)
                return NotFound();

            var usuarioLogado = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            if (comentario.UsuarioId != usuarioLogado.Id && !isAdmin)
                return Forbid();

            _context.ComentariosJogo.Remove(comentario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Jogo", new { jogoId = comentario.JogoId });
        }

        // comentarios gerados 
        public async Task<IActionResult> Realizadas()
        {
            var user = await _userManager.GetUserAsync(User);

            var comentarios = await _context.ComentariosJogo
                .Include(c => c.Jogo)
                .Where(c => c.UsuarioId == user.Id)
                .OrderByDescending(c => c.Data)
                .ToListAsync();

            // Dicionário de curtidas por comentário
            var curtidas = await _context.ComentarioCurtidas
                .Where(cc => comentarios.Select(c => c.Id).Contains(cc.ComentarioId))
                .GroupBy(cc => cc.ComentarioId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            ViewBag.CurtidasPorComentario = curtidas;

            return View("~/Views/MeusPedidos/Realizadas.cshtml", comentarios);
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var comentario = await _context.ComentariosJogo
                .Include(c => c.Jogo)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comentario == null)
                return NotFound();

            var usuario = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            if (comentario.UsuarioId != usuario.Id && !isAdmin)
                return Forbid();

            return View("~/Views/MeusPedidos/Editar.cshtml", comentario);
        }

        // ✅ Método POST chamado quando o formulário da edição é enviado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ComentarioJogo model)
        {
            if (!ModelState.IsValid)
            {
                model.Jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.JogoId == model.JogoId);
                TempData["Erro"] = "Preencha todos os campos corretamente.";
                return View("~/Views/MeusPedidos/Editar.cshtml", model);
            }

            var comentario = await _context.ComentariosJogo.FindAsync(model.Id);
            if (comentario == null)
                return NotFound();

            var usuario = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            if (comentario.UsuarioId != usuario.Id && !isAdmin)
                return Forbid();

            comentario.Texto = string.IsNullOrWhiteSpace(model.Texto) ? null : model.Texto.Trim();
            comentario.Avaliacao = model.Avaliacao;
            comentario.Data = DateTime.Now;

            _context.ComentariosJogo.Update(comentario);
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Comentário atualizado com sucesso!";
            return RedirectToAction("Realizadas");
        }

        [HttpPost]
        public async Task<IActionResult> Curtir(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var curtidaExistente = await _context.ComentarioCurtidas
                .FirstOrDefaultAsync(c => c.ComentarioId == id && c.UsuarioId == user.Id);

            if (curtidaExistente != null)
            {
                // DESCURTIR
                _context.ComentarioCurtidas.Remove(curtidaExistente);
                await _context.SaveChangesAsync();

                var totalCurtidas = await _context.ComentarioCurtidas.CountAsync(c => c.ComentarioId == id);

                return Json(new
                {
                    sucesso = true,
                    curtido = false,
                    curtidas = totalCurtidas
                });
            }

            // CURTIR
            var novaCurtida = new ComentarioCurtida
            {
                ComentarioId = id,
                UsuarioId = user.Id
            };

            _context.ComentarioCurtidas.Add(novaCurtida);
            await _context.SaveChangesAsync();

            var novasCurtidas = await _context.ComentarioCurtidas.CountAsync(c => c.ComentarioId == id);



            return Json(new
            {
                sucesso = true,
                curtido = true,
                curtidas = novasCurtidas
            });
        }


    }

}


