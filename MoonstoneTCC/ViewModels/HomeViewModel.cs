using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Jogo> JogosPreferidos { get; set; }

        public List<(int JogoId, int Posicao)> RankingMaisVendidos { get; set; }

        public List<ListaJogo> ListasDoUsuario { get; set; }

    }
}
