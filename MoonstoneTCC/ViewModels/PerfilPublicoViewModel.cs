namespace MoonstoneTCC.ViewModels
{
    using MoonstoneTCC.Models;
    using System.Collections.Generic;

    public class PerfilPublicoViewModel
    {
        public string UsuarioId { get; set; }
        public string NomeUsuario { get; set; }
        public string FotoUrl { get; set; } // Se você quiser usar imagem de perfil

        public List<string> Interesses { get; set; } = new();
        public List<ComentarioJogo> Comentarios { get; set; } = new();
        public List<ListaJogo> ListasPublicas { get; set; } = new();
        public bool EstaSeguindo { get; set; }
        public int TotalSeguidores { get; set; }
        public int TotalSeguindo { get; set; }
        public string DesenvolvedoraId { get; set; }
        public List<AvaliacaoJogo> AvaliacoesLikes { get; set; } = new();
        public List<AvaliacaoJogo> AvaliacoesDislikes { get; set; } = new();
        public List<Jogo> FavoritosPublicos { get; set; } = new();



    }
}
