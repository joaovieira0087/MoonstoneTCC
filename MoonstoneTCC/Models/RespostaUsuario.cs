using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MoonstoneTCC.Models
{
    public class RespostaUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        [Required]
        public int ComunicadoId { get; set; }

        [ForeignKey("ComunicadoId")]
        public Comunicado Comunicado { get; set; }

        public string? TextoResposta { get; set; } // Para perguntas abertas
        public string? OpcaoEscolhida { get; set; } // Para enquetes
        public DateTime DataResposta { get; set; } = DateTime.Now;
    }
}
