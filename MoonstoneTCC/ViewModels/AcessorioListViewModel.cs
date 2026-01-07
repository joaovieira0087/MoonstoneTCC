using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class AcessorioListViewModel
    {
        public IEnumerable<Acessorio> Acessorios { get; set; }
        public string CategoriaAtual { get; set; }
    }
}
