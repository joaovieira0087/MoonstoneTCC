using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    public class Favorito
    {
        [Key]
        public int Id { get; set; }

        public int JogoId { get; set; }
        [ForeignKey("JogoId")]
        public Jogo Jogo { get; set; }

        public string UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        public bool EPublico { get; set; } = false;
        public string? TagFavorito { get; set; }

    }
}

