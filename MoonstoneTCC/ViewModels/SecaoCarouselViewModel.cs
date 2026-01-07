using System.Collections.Generic;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class SecaoCarouselViewModel
    {
        public string SectionId { get; set; }          // único na página (ex.: "sec-continuar", "sec-vnao")
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }

        public IEnumerable<Jogo> Jogos { get; set; } = new List<Jogo>();
        public int ItensPorPagina { get; set; } = 4;

        // Visual
        public string Badge { get; set; }              // mostra o mesmo badge em todos os cards (ex.: "Continuar vendo")
        public bool Destaque { get; set; } = false;

        // Ranking (para “Mais vendidos”)
        public Dictionary<int, int> Ranking { get; set; } = new();

        // Favoritos/Autenticação
        public bool UsuarioAutenticado { get; set; }
        public HashSet<int> FavoritosIds { get; set; } = new();
    }
}

