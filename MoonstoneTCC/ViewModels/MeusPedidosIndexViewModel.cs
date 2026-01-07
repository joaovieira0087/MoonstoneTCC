namespace MoonstoneTCC.ViewModels
{
    using System.Collections.Generic;
    using MoonstoneTCC.Models;

    public class MeusPedidosIndexViewModel
    {
        public List<Pedido> Pedidos { get; set; } = new();
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }

}
