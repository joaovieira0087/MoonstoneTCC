using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    [Table("AcoesAdmin")]
    public class AcaoAdmin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public string Acao { get; set; }

        public DateTime DataHora { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }
    }
}
