using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.ViewModels;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class FavoritoController : Controller
    {
        private readonly IFavoritoRepository _favoritoRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public FavoritoController(IFavoritoRepository favoritoRepo, UserManager<IdentityUser> userManager,AppDbContext context)
        {
            _favoritoRepo = favoritoRepo;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Adicionar(int jogoId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (_favoritoRepo.JaAdicionado(user.Id, jogoId))
            {
                TempData["Mensagem"] = "Este jogo já está nos seus favoritos! <a href='/Favorito'>Clique aqui para ver</a>.";
                TempData["TipoMensagem"] = "info"; // Azul
            }
            else
            {
                _favoritoRepo.Adicionar(user.Id, jogoId);
                TempData["Mensagem"] = "Jogo adicionado aos favoritos com sucesso! <a href='/Favorito'>Ver favoritos</a>.";
                TempData["TipoMensagem"] = "success"; // Verde
            }

            return RedirectToAction("Details", "Jogo", new { jogoId });
        }

        public async Task<IActionResult> Remover(int jogoId)
        {
            var user = await _userManager.GetUserAsync(User);
            _favoritoRepo.Remover(user.Id, jogoId);
            TempData["Mensagem"] = "Jogo removido dos favoritos.";
            TempData["TipoMensagem"] = "warning"; // Amarelo
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(
        string ordem,
        string visibilidade,
        string plataforma,
        string tag)
        {
            var user = await _userManager.GetUserAsync(User);

            var favoritos = _favoritoRepo.GetFavoritosComDetalhesDoUsuario(user.Id).AsQueryable();

            if (visibilidade == "publico")
                favoritos = favoritos.Where(f => f.EPublico);
            else if (visibilidade == "privado")
                favoritos = favoritos.Where(f => !f.EPublico);

            if (!string.IsNullOrEmpty(plataforma))
                favoritos = favoritos.Where(f => f.Jogo.Plataformas.Contains(plataforma));

            if (!string.IsNullOrEmpty(tag))
                favoritos = favoritos.Where(f => f.TagFavorito == tag);

            favoritos = ordem switch
            {
                "alfabetica" => favoritos.OrderBy(f => f.Jogo.Nome),
                "recentes" => favoritos.OrderByDescending(f => f.Jogo.JogoId),
                "antigos" => favoritos.OrderBy(f => f.Jogo.JogoId),
                _ => favoritos
            };

            var favoritosList = favoritos.ToList();

            var plataformas = _context.Jogos
                .Select(j => j.Plataformas)
                .Distinct()
                .ToList();

            var todasTags = _context.Favoritos
            .Where(f => f.UsuarioId == user.Id && !string.IsNullOrEmpty(f.TagFavorito))
            .Select(f => f.TagFavorito)
            .Distinct()
            .ToList();

            ViewBag.TodasTags = todasTags;


            return View(new FavoritoIndexViewModel
            {
                Favoritos = favoritosList,
                FiltroOrdem = ordem,
                FiltroVisibilidade = visibilidade,
                FiltroPlataforma = plataforma,
                FiltroTag = tag,
                PlataformasDisponiveis = plataformas
            });
        }


        [HttpPost]
        public async Task<IActionResult> AtualizarTag(int jogoId, string tag, string novaTag)
        {
            var user = await _userManager.GetUserAsync(User);
            var favorito = _favoritoRepo.ObterFavorito(user.Id, jogoId);

            if (favorito == null)
                return NotFound();

            // Se a tag for "nova", usamos o valor do campo novaTag
            favorito.TagFavorito = tag == "nova" && !string.IsNullOrWhiteSpace(novaTag) ? novaTag.Trim() : tag;
            _favoritoRepo.Salvar();

            TempData["Mensagem"] = "Categoria atualizada com sucesso!";
            TempData["TipoMensagem"] = "success";

            return RedirectToAction("Index");
        }







        [HttpPost]
        public async Task<IActionResult> AlternarVisibilidade(int jogoId)

        {
            var user = await _userManager.GetUserAsync(User);
            var favorito = _favoritoRepo.ObterFavorito(user.Id, jogoId);

            if (favorito == null)
                return NotFound();

            favorito.EPublico = !favorito.EPublico;
            _favoritoRepo.Salvar();

            TempData["Mensagem"] = favorito.EPublico
                ? "Esse jogo agora está como favorito público."
                : "Esse jogo agora é um favorito privado.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ImportarTodos(string usuarioId)
        {
            var user = await _userManager.GetUserAsync(User);

            var favoritosPublicos = _favoritoRepo
                .GetFavoritosPublicosDoUsuario(usuarioId)
                .Select(f => f.JogoId)
                .ToList();

            foreach (var jogoId in favoritosPublicos)
            {
                if (!_favoritoRepo.JaAdicionado(user.Id, jogoId))
                {
                    _favoritoRepo.Adicionar(user.Id, jogoId);
                }
            }

            TempData["Mensagem"] = "Favoritos importados com sucesso!";
            TempData["TipoMensagem"] = "success";

            return RedirectToAction("Index");
        }



    }
}
