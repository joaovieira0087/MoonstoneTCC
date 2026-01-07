using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    public class HistoricoVisualizacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        [Required]
        public int JogoId { get; set; }

        [ForeignKey("JogoId")]
        public Jogo Jogo { get; set; }

        [Display(Name = "Data da Visualização")]
        public DateTime DataVisualizacao { get; set; } = DateTime.Now;
    }
}
