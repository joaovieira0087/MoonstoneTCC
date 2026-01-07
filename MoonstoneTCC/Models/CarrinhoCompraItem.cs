using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    [Table("CarrinhoCompraItens")]
    public class CarrinhoCompraItem
    {
        public int CarrinhoCompraItemId { get; set; }

        public int? JogoId { get; set; }
        public virtual Jogo Jogo { get; set; }

        public int? AcessorioId { get; set; }
        public virtual Acessorio Acessorio { get; set; }

        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecoUnitario { get; set; }

        [StringLength(200)]
        public string CarrinhoCompraId { get; set; }
    }
}
