using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    public class ComentarioJogo
    {
        public int Id { get; set; }

        [BindNever]
        public string UsuarioId { get; set; }

        [Required]
        public int JogoId { get; set; }

        [StringLength(1000)]
        public string Texto { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        [ForeignKey("UsuarioId")]
        public virtual IdentityUser Usuario { get; set; }

        [ForeignKey("JogoId")]
        public virtual Jogo Jogo { get; set; }

        [Range(1, 5)]
        public int Avaliacao { get; set; }
    }
}
