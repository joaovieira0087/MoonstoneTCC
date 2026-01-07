using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    public class CartaoCredito
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser Usuario { get; set; }

        [Required]
        [StringLength(20)]
        public string NumeroParcial { get; set; } // Apenas os 4 últimos dígitos

        [Required]
        [StringLength(5)]
        public string Validade { get; set; } // MM/AA

        [Required]
        [StringLength(100)]
        public string NomeTitular { get; set; }

        [Required]
        [StringLength(6)]
        public string CodigoVerificacao { get; set; } // Criado pelo usuário

        public bool CartaoPadrao { get; set; }
    }
}

