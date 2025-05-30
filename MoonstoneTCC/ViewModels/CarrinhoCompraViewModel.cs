using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class CarrinhoCompraViewModel
    {
        public CarrinhoCompra CarrinhoCompra { get; set; }
        public decimal CarrinhoCompraTotal { get; set; }

        
        public string CodigoCupom { get; set; }
        public decimal Desconto { get; set; }           
        public decimal TotalComDesconto { get; set; }    
    }
}
