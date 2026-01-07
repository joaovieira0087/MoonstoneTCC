using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class JogoListViewModel
    {
        public IEnumerable<Jogo> Jogos { get; set; }
        public IEnumerable<Acessorio> Acessorios { get; set; }
        public string CategoriaAtual { get; set; }
    }
}
