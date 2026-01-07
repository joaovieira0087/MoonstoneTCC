using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    public class Pedido
    {
        public int PedidoId { get; set; }

        public string UserId { get; set; } 

        [ForeignKey("UserId")]
        public IdentityUser Usuario { get; set; }

        public string CodigoPedido { get; set; }

        [Required(ErrorMessage = "Informe o nome")]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o sobrenome")]
        [StringLength(50)]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "Informe o seu endereço")]
        [StringLength(100)]
        [Display(Name = "Endereço")]
        public string Endereco1 { get; set; }

        [StringLength(100)]
        [Display(Name = "Complemento")]
        public string Endereco2 { get; set; }

        [Required(ErrorMessage = "Informe o seu CEP")]
        [Display(Name = "CEP")]
        [StringLength(10, MinimumLength = 8)]
        public string Cep { get; set; }

        [StringLength(10)]
        public string Estado { get; set; }

        [StringLength(50)]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "Infome o seu Telefone")]
        [StringLength(25)]
        [DataType(DataType.PhoneNumber)]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Informe o email.")]
        [StringLength(50)]
        public string Email { get; set; }


        [ScaffoldColumn(false)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total do Pedido")]
        public decimal PedidoTotal { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Itens no Pedido")]
        public int TotalItensPedido { get; set; }

        [Display(Name = "Data do Pedido")]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime PedidoEnviado { get; set; }

        [Display(Name = "Data Envio Pedido")]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? PedidoEntregueEm { get; set; }

        public List<PedidoDetalhe> PedidoItens { get; set; }


        // pos pedido 

        [Display(Name = "Status do Pedido")]
        public string StatusPedido { get; set; }

        [Display(Name = "Motivo do Cancelamento")]
        public string MotivoCancelamento { get; set; }

        [Display(Name = "Data de Cancelamento")]
        public DateTime? DataCancelamento { get; set; }

        public CancelamentoPedido? Cancelamento { get; set; }

        public FormaPagamentoPedido? FormaPagamento { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? ValorPagoCarteira { get; set; }

        // frete 
        public decimal ValorFrete { get; set; }      
        public int PrazoEntregaDias { get; set; }



    }
}
