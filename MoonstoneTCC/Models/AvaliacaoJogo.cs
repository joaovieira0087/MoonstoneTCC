using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    public class AvaliacaoJogo
    {
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }

        [Required]
        public int JogoId { get; set; }
        public Jogo Jogo { get; set; }

        public bool? Gostou { get; set; } // true = like, false = dislike, null = nenhum
    }

}
