// ViewModels/HomePersonalizadaViewModel.cs
using System.Collections.Generic;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class HomePersonalizadaViewModel
    {
        public IEnumerable<Jogo> FavoritosDoUsuario { get; set; } = new List<Jogo>();
        public IEnumerable<Jogo> ContinuarVendo { get; set; } = new List<Jogo>(); // do histórico
        public IEnumerable<Jogo> ViuMasNaoComprou { get; set; } = new List<Jogo>(); // destaque
        public IEnumerable<Jogo> RecomendadosPorCategoria { get; set; } = new List<Jogo>(); // com base no histórico/favoritos
        public IEnumerable<Jogo> MaisVendidosSemana { get; set; } = new List<Jogo>(); // já tinha
        public IEnumerable<Jogo> EmPromocao { get; set; } = new List<Jogo>(); // opcional (promoções)
        public Dictionary<int, int> RankingMaisVendidos { get; set; } = new(); // jogoId -> posição
        public List<DiscoverRowVM> DiscoverRows { get; set; } = new();
        public bool UsuarioLogado { get; set; }
        public string NomeUsuario { get; set; }
    }
}

