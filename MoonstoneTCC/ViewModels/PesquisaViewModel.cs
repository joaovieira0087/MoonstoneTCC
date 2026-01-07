using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class PesquisaViewModel
    {
        public IEnumerable<Jogo> Jogos { get; set; }
        public IEnumerable<Acessorio> Acessorios { get; set; }
        public string TermoPesquisado { get; set; }
    }
}

