using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Areas.Admin.Services
{
    public class GraficoVendasService
    {
        private readonly AppDbContext context;

        public GraficoVendasService(AppDbContext context)
        {
            this.context = context;
        }

        public List<JogoGrafico> GetVendasJogos(int dias = 0)
        {
            DateTime? data = dias > 0 ? DateTime.Now.AddDays(-dias) : (DateTime?)null;

            var jogos = (from pd in context.PedidoDetalhes
                         join j in context.Jogos on pd.JogoId equals j.JogoId
                         where data == null || pd.Pedido.PedidoEnviado >= data
                         group pd by new { pd.JogoId, j.Nome }
                         into g
                         select new
                         {
                             JogoNome = g.Key.Nome,
                             JogosQuantidade = g.Sum(j => j.Quantidade),
                             JogosValorTotal = g.Sum(a => a.Preco * a.Quantidade)
                         });


            var lista = new List<JogoGrafico>();

            foreach (var item in jogos)
            {
                var jogo = new JogoGrafico
                {
                    JogoNome = item.JogoNome,
                    JogosQuantidade = item.JogosQuantidade,
                    JogosValorTotal = item.JogosValorTotal
                };
                lista.Add(jogo);
            }

            return lista;
        }

        public List<JogoGrafico> GetVendasJogosPersonalizado(DateTime startDate, DateTime endDate)
        {
            var jogos = (from pd in context.PedidoDetalhes
                         join j in context.Jogos on pd.JogoId equals j.JogoId
                         where pd.Pedido.PedidoEnviado >= startDate && pd.Pedido.PedidoEnviado <= endDate
                         group pd by new { pd.JogoId, j.Nome }
                         into g
                         select new
                         {
                             JogoNome = g.Key.Nome,
                             JogosQuantidade = g.Sum(j => j.Quantidade),
                             JogosValorTotal = g.Sum(a => a.Preco * a.Quantidade)
                         });

            return jogos.Select(item => new JogoGrafico
            {
                JogoNome = item.JogoNome,
                JogosQuantidade = item.JogosQuantidade,
                JogosValorTotal = item.JogosValorTotal
            }).ToList();
        }


    }
}