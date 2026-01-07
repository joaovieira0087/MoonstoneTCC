using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Repositories.Interfaces;

namespace MoonstoneTCC.Components
{
    public class MaisCompradosViewComponent : ViewComponent
    {
        private readonly IJogoRepository _jogoRepository;

        public MaisCompradosViewComponent(IJogoRepository jogoRepository)
        {
            _jogoRepository = jogoRepository;
        }

        public IViewComponentResult Invoke(int? categoriaId = null, int quantidade = 5)
        {
            var jogos = categoriaId == null
                ? _jogoRepository.GetJogosMaisComprados(quantidade)
                : _jogoRepository.GetJogosMaisCompradosPorCategoria(categoriaId.Value, quantidade);

            return View(jogos);
        }
    }
}
