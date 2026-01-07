// Models/CarteiraUsuario.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    public class CarteiraUsuario
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; }

        // Concurrency safe
        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;

        public List<TransacaoCarteira> Transacoes { get; set; } = new();
    }

    public class TransacaoCarteira
    {
        public int Id { get; set; }

        public int CarteiraUsuarioId { get; set; }
        public CarteiraUsuario Carteira { get; set; } = default!;

        public DateTime Data { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; } // positivo para crédito, negativo para débito

        public TipoTransacaoCarteira Tipo { get; set; }

        // Amarre com pedido quando fizer pagamento/estorno
        public int? PedidoId { get; set; }

        // Pra auditoria/histórico
        [MaxLength(200)]
        public string? Referencia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoApos { get; set; }
    }

    public enum TipoTransacaoCarteira
    {
        Deposito = 1,
        DebitoPagamentoPedido = 2,
        EstornoCancelamento = 3,
        Ajuste = 4
    }

    // Opcional: gravar no Pedido
    public enum FormaPagamentoPedido
    {
        Cartao = 1,
        Carteira = 2
    }
}

