// ViewModels/SecaoGridViewModel.cs
using System.Collections.Generic;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class SecaoGridViewModel
    {
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public IEnumerable<Jogo> Jogos { get; set; } = new List<Jogo>();
        public bool Destaque { get; set; }
        public Dictionary<int, int> Ranking { get; set; } = new(); // jogoId -> posição (opcional)
    }
}
