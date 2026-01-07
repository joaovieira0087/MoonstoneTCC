using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class CardJogoViewModel
    {
        public Jogo Jogo { get; set; }

        // UI
        public string Badge { get; set; }            // "Continuar vendo", "Você viu...", "Favorito", "Recomendado", etc.
        public bool Destaque { get; set; }           // true = card com brilho/realce
        public int? RankingPosicao { get; set; }     // 1, 2, 3... (para "Mais vendidos")

        // Favorito
        public bool UsuarioAutenticado { get; set; }
        public bool EhFavorito { get; set; }         // usado para saber se mostra "Favoritar" ou "Remover"
    }
}

