using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.ViewModels;

namespace MoonstoneTCC.Controllers
{
    public class JogoController : Controller
    {
        private readonly IJogoRepository _jogoRepository;

        public JogoController(IJogoRepository jogoRepository)
        {
            _jogoRepository = jogoRepository;
        }

        public IActionResult List(string categoria)
        {
            IEnumerable<Jogo> jogos;
            string categoriaAtual;

            if (string.IsNullOrEmpty(categoria))
            {
                jogos = _jogoRepository.Jogos.OrderBy(j => j.JogoId);
                categoriaAtual = "Jogos"; // Nome padrão
            }
            else
            {
                jogos = _jogoRepository.Jogos
                    .Where(j => j.Categoria.CategoriaNome.Equals(categoria))
                    .OrderBy(j => j.Nome);
                categoriaAtual = categoria; // Nome da categoria atual
            }

            // Atualizar o título dinâmico no dropdown
            ViewData["CategoriaAtual"] = categoriaAtual;

            var jogosListViewModel = new JogoListViewModel
            {
                Jogos = jogos,
                CategoriaAtual = categoriaAtual
            };

            return View(jogosListViewModel);
        }
        public IActionResult Details(int jogoId)
        {
            var jogo = _jogoRepository.Jogos.FirstOrDefault(j=> j.JogoId == jogoId);
            return View (jogo);
        }

        public ViewResult Search(string searchString)
        {
            // Sanitize and normalize the search string
            searchString = searchString?.Trim().ToLower();

            IEnumerable<Jogo> jogos;
            string categoriaAtual;

            if (string.IsNullOrEmpty(searchString))
            {
                jogos = _jogoRepository.Jogos.OrderBy(p => p.Nome);
                categoriaAtual = "Todos os Jogos";
            }
            else
            {
                jogos = _jogoRepository.Jogos
                 .Where(p => p.Nome.ToLower().Contains(searchString)
                         || (p.Categoria != null && p.Categoria.CategoriaNome.ToLower().Contains(searchString))
                         || (!string.IsNullOrEmpty(p.DescricaoCurta) && p.DescricaoCurta.ToLower().Contains(searchString))
                         || (!string.IsNullOrEmpty(p.DescricaoDetalhada) && p.DescricaoDetalhada.ToLower().Contains(searchString))
                         || (!string.IsNullOrEmpty(p.Genero) && p.Genero.ToLower().Contains(searchString)) // Pesquisa por gênero
                         || (p.Preco.ToString("F2").Contains(searchString)) // Pesquisa por preço (convertido para string)
                         || (p.Plataformas != null && p.Plataformas.ToLower().Contains(searchString)) // Pesquisa por plataformas
                 )
                 .OrderBy(p => p.Nome);




                categoriaAtual = jogos.Any() ? $"Resultados para '{searchString}'" : "Nenhum Jogo foi encontrado";
            }

            return View("~/Views/Jogo/List.cshtml", new JogoListViewModel
            {
                Jogos = jogos,
                CategoriaAtual = categoriaAtual
            });
        }

        public IActionResult Recomendados(int jogoId)
        {
            var jogosRecomendados = _jogoRepository.GetJogosRecomendados(jogoId, 4);
            return PartialView("_JogosRecomendados", jogosRecomendados);
        }




    }
}
