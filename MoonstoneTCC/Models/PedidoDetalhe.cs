using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    public class PedidoDetalhe
    {
        public int PedidoDetalheId { get; set; }

        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }

        public int? JogoId { get; set; }
        public virtual Jogo Jogo { get; set; }

        public int? AcessorioId { get; set; }
        public virtual Acessorio Acessorio { get; set; }

        public int Quantidade { get; set; }

        public decimal Preco { get; set; }
    }

}
