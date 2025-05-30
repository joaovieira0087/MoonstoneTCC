using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class PedidoJogoViewModel
    {
        public Pedido Pedido { get; set; }
        public IEnumerable<PedidoDetalhe> PedidoDetalhes { get; set; }
    }
}
