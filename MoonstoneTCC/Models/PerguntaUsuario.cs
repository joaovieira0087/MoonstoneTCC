using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    public class PerguntaUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(1000)]
        public string Mensagem { get; set; }

        public DateTime DataEnvio { get; set; } = DateTime.Now;

        public string? RespostaAdmin { get; set; }

        public DateTime? DataResposta { get; set; }

        public bool Respondido { get; set; } = false;
    }
}
