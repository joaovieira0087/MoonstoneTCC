using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;

namespace MoonstoneTCC.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly CarrinhoCompra _carrinhoCompra;

        public PedidoRepository(AppDbContext appDbContext, CarrinhoCompra carrinhoCompra)
        {
            _appDbContext = appDbContext;
            _carrinhoCompra = carrinhoCompra;
        }

        public void CriarPedido(Pedido pedido)
        {
            pedido.PedidoEnviado = DateTime.Now;
            pedido.StatusPedido = "Pagamento Confirmado"; // status inicial
            _appDbContext.Pedidos.Add(pedido);
            _appDbContext.SaveChanges();

            var carrinhoCompraItens = _carrinhoCompra.CarrinhoCompraItems;

            foreach (var carrinhoItem in carrinhoCompraItens)
            {
                // Tratamento para JOGO
                if (carrinhoItem.Jogo != null)
                {
                    var jogo = _appDbContext.Jogos.FirstOrDefault(j => j.JogoId == carrinhoItem.Jogo.JogoId);
                    if (jogo == null)
                        throw new Exception("Jogo não encontrado.");

                    if (jogo.QuantidadeEstoque < carrinhoItem.Quantidade)
                        throw new Exception($"Estoque insuficiente para o jogo {jogo.Nome}");

                    jogo.QuantidadeEstoque -= carrinhoItem.Quantidade;

                    // Se voltou ao estoque, notifica quem solicitou
                    if (jogo.QuantidadeEstoque > 0)
                    {
                        var avisos = _appDbContext.AvisosEstoque
                            .Where(a => a.JogoId == jogo.JogoId && !a.Avisado)
                            .ToList();

                        foreach (var aviso in avisos)
                        {
                            aviso.Avisado = true;
                            // Você pode adicionar lógica para criar notificação visual aqui
                        }
                    }

                    var pedidoDetail = new PedidoDetalhe()
                    {
                        Quantidade = carrinhoItem.Quantidade,
                        JogoId = jogo.JogoId,
                        PedidoId = pedido.PedidoId,
                        Preco = carrinhoItem.PrecoUnitario
                    };

                    _appDbContext.PedidoDetalhes.Add(pedidoDetail);
                }
                // Tratamento para ACESSÓRIO
                else if (carrinhoItem.Acessorio != null)
                {
                    var acessorio = _appDbContext.Acessorios.FirstOrDefault(a => a.AcessorioId == carrinhoItem.Acessorio.AcessorioId);
                    if (acessorio == null)
                        throw new Exception("Acessório não encontrado.");

                    if (acessorio.QuantidadeEstoque < carrinhoItem.Quantidade)
                        throw new Exception($"Estoque insuficiente para o acessório {acessorio.Nome}");

                    acessorio.QuantidadeEstoque -= carrinhoItem.Quantidade;

                    // Se voltou ao estoque, notifica quem solicitou
                    if (acessorio.QuantidadeEstoque > 0)
                    {
                        var avisos = _appDbContext.AvisosEstoque
                            .Where(a => a.AcessorioId == acessorio.AcessorioId && !a.Avisado)
                            .ToList();

                        foreach (var aviso in avisos)
                        {
                            aviso.Avisado = true;
                        }
                    }

                    var pedidoDetail = new PedidoDetalhe()
                    {
                        Quantidade = carrinhoItem.Quantidade,
                        AcessorioId = acessorio.AcessorioId,
                        PedidoId = pedido.PedidoId,
                        Preco = carrinhoItem.PrecoUnitario
                    };

                    _appDbContext.PedidoDetalhes.Add(pedidoDetail);
                }
            }

            _appDbContext.SaveChanges();

            // Gerar código do pedido no padrão "MOON-250731-00001"
            pedido.CodigoPedido = $"MOON-{DateTime.Now:yyMMdd}-{pedido.PedidoId:D5}";
            _appDbContext.SaveChanges();
        }
    }
}
