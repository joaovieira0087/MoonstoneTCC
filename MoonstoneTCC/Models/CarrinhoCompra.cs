using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;

namespace MoonstoneTCC.Models
{
    public class CarrinhoCompra
    {
        private readonly AppDbContext _context;

        public CarrinhoCompra(AppDbContext context)
        {
            _context = context;
        }

        public string CarrinhoCompraId { get; set; }

        public List<CarrinhoCompraItem> CarrinhoCompraItems { get; set; }

        public static CarrinhoCompra GetCarrinho(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<AppDbContext>();

            string carrinhoId = session.GetString("CarrinhoId") ?? Guid.NewGuid().ToString();
            session.SetString("CarrinhoId", carrinhoId);

            return new CarrinhoCompra(context) { CarrinhoCompraId = carrinhoId };
        }

        // ✅ Adicionar jogo
        public void AdicionarAoCarrinho(Jogo jogo, int quantidade = 1)
        {
            var precoFinal = jogo.PrecoPromocional ?? jogo.Preco;

            var itemAtual = _context.CarrinhoCompraItens
                .FirstOrDefault(c => c.JogoId == jogo.JogoId && c.CarrinhoCompraId == CarrinhoCompraId);

            if (itemAtual == null)
            {
                itemAtual = new CarrinhoCompraItem
                {
                    CarrinhoCompraId = CarrinhoCompraId,
                    JogoId = jogo.JogoId,
                    Quantidade = quantidade,
                    PrecoUnitario = precoFinal
                };
                _context.CarrinhoCompraItens.Add(itemAtual);
            }
            else
            {
                itemAtual.Quantidade += quantidade;
            }

            _context.SaveChanges();
        }

        // ✅ Adicionar acessório
        public void AdicionarAoCarrinho(Acessorio acessorio, int quantidade = 1)
        {
            var precoFinal = acessorio.PrecoPromocional ?? acessorio.Preco;

            var itemAtual = _context.CarrinhoCompraItens
                .FirstOrDefault(c => c.AcessorioId == acessorio.AcessorioId && c.CarrinhoCompraId == CarrinhoCompraId);

            if (itemAtual == null)
            {
                itemAtual = new CarrinhoCompraItem
                {
                    CarrinhoCompraId = CarrinhoCompraId,
                    AcessorioId = acessorio.AcessorioId,
                    Quantidade = quantidade,
                    PrecoUnitario = precoFinal
                };
                _context.CarrinhoCompraItens.Add(itemAtual);
            }
            else
            {
                itemAtual.Quantidade += quantidade;
            }

            _context.SaveChanges();
        }

        // ✅ Remover jogo
        public int RemoverDoCarrinho(Jogo jogo)
        {
            var item = _context.CarrinhoCompraItens
                .FirstOrDefault(i => i.JogoId == jogo.JogoId && i.CarrinhoCompraId == CarrinhoCompraId);

            return RemoverItem(item);
        }

        // ✅ Remover acessório
        public int RemoverDoCarrinho(Acessorio acessorio)
        {
            var item = _context.CarrinhoCompraItens
                .FirstOrDefault(i => i.AcessorioId == acessorio.AcessorioId && i.CarrinhoCompraId == CarrinhoCompraId);

            return RemoverItem(item);
        }

        private int RemoverItem(CarrinhoCompraItem item)
        {
            int quantidadeLocal = 0;

            if (item != null)
            {
                if (item.Quantidade > 1)
                {
                    item.Quantidade--;
                    quantidadeLocal = item.Quantidade;
                }
                else
                {
                    _context.CarrinhoCompraItens.Remove(item);
                }

                _context.SaveChanges();
            }

            return quantidadeLocal;
        }

        public List<CarrinhoCompraItem> GetCarrinhoCompraItens()
        {
            return CarrinhoCompraItems ??= _context.CarrinhoCompraItens
                .Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                .Include(c => c.Jogo)
                .Include(c => c.Acessorio)
                .ToList();
        }

        public void LimparCarrinho()
        {
            var carrinhoItens = _context.CarrinhoCompraItens
                .Where(c => c.CarrinhoCompraId == CarrinhoCompraId);

            _context.CarrinhoCompraItens.RemoveRange(carrinhoItens);
            _context.SaveChanges();
        }

        public decimal GetCarrinhoCompraTotal()
        {
            return _context.CarrinhoCompraItens
                .Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                .Select(c => c.PrecoUnitario * c.Quantidade)
                .Sum();
        }
    }
}
