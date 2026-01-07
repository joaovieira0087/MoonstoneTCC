using MoonstoneTCC.Models;

public class CarrinhoCompraViewModel
{
    public CarrinhoCompra CarrinhoCompra { get; set; }
    public decimal CarrinhoCompraTotal { get; set; }

    // Cupom
    public string CodigoCupom { get; set; }
    public decimal Desconto { get; set; }
    public decimal TotalComDesconto { get; set; }

    // Frete
    public string CepDestino { get; set; }
    public decimal ValorFrete { get; set; }      // deixe decimal (não nulo) e default 0
    public int PrazoFreteDias { get; set; }
    public string RegiaoDestino { get; set; }

    // Total final para exibir
    public decimal TotalFinal => TotalComDesconto + ValorFrete;

    // (o seu campo de economia pode ficar)
    public decimal EconomiaTotal { get; set; }
}
