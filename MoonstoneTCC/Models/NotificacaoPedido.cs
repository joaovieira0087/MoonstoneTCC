using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    public class NotificacaoPedido
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }

        public int PedidoId { get; set; }

        public string Mensagem { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public bool Lida { get; set; } = false;

        public Pedido Pedido { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }
    }

}
