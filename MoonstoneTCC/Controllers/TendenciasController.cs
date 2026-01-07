using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Repositories.Interfaces;

namespace MoonstoneTCC.Controllers
{
    public class TendenciasController : Controller
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public TendenciasController(IJogoRepository jogoRepository, ICategoriaRepository categoriaRepository)
        {
            _jogoRepository = jogoRepository;
            _categoriaRepository = categoriaRepository;
        }

        public IActionResult List(int? categoriaId)
        {
            ViewBag.Categorias = _categoriaRepository.Categorias;

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                var categoria = _categoriaRepository.Categorias.FirstOrDefault(c => c.CategoriaId == categoriaId.Value);
                ViewBag.Titulo = $"Mais Comprados em {categoria?.CategoriaNome}";
                ViewBag.CategoriaSelecionada = categoriaId.Value;

                var jogos = _jogoRepository.GetJogosMaisCompradosPorCategoria(categoriaId.Value, 10);
                return View(jogos);
            }
            else
            {
                ViewBag.Titulo = "Mais Comprados no Geral";
                ViewBag.CategoriaSelecionada = 0;
                var jogos = _jogoRepository.GetJogosMaisComprados(10);
                return View(jogos);
            }
        }

        [HttpGet]
        public JsonResult MaisBuscados()
        {
            var jogos = _jogoRepository.GetJogosMaisBuscados(6)
                .Select(j => new {
                    id = j.JogoId,
                    nome = j.Nome,
                    imagemUrl = j.ImagemUrl,
                    genero = j.Genero
                }).ToList();

            return Json(jogos);
        }

    }
}
