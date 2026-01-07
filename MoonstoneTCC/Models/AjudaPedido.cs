using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    public class AjudaPedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [ForeignKey("PedidoId")]
        public Pedido Pedido { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoProblema { get; set; }

        [StringLength(1000)]
        public string? Descricao { get; set; }

        public DateTime DataEnvio { get; set; } = DateTime.Now;

        public string? RespostaAdmin { get; set; }

        public DateTime? DataResposta { get; set; }

        public bool Resolvido { get; set; } = false;
    }
}
