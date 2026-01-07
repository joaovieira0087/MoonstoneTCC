namespace MoonstoneTCC.Services
{
    public record CotacaoFrete(decimal Valor, int PrazoDias, string Regiao, string? Observacao = null);

    public interface ICepFreteService
    {
        CotacaoFrete Calcular(string cep, decimal subtotal, int quantidadeItens);
    }
}
