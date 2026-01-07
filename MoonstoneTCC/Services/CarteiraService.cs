// Services/CarteiraService.cs
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Services
{
    public interface ICarteiraService
    {
        Task<CarteiraUsuario> ObterOuCriarAsync(string userId);
        Task<CarteiraUsuario> ObterAsync(string userId);
        Task DepositarAsync(string userId, decimal valor, string? referencia = null);
        Task DebitarParaPedidoAsync(string userId, decimal valor, int pedidoId);
        Task EstornarPedidoAsync(string userId, decimal valor, int pedidoId);
        Task<bool> JaEstornadoAsync(int pedidoId);
    }

    public class CarteiraService : ICarteiraService
    {
        private readonly AppDbContext _db;

        public CarteiraService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CarteiraUsuario> ObterOuCriarAsync(string userId)
        {
            var c = await _db.Carteiras.FirstOrDefaultAsync(x => x.UserId == userId);
            if (c != null) return c;

            c = new CarteiraUsuario { UserId = userId, Saldo = 0m };
            _db.Carteiras.Add(c);
            await _db.SaveChangesAsync();
            return c;
        }

        public async Task<CarteiraUsuario> ObterAsync(string userId)
        {
            // Tenta obter, se não existir, cria automaticamente para evitar NullReference na View
            var c = await _db.Carteiras
                .Include(x => x.Transacoes.OrderByDescending(t => t.Data).Take(50))
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (c == null) return await ObterOuCriarAsync(userId);

            return c;
        }

        public async Task DepositarAsync(string userId, decimal valor, string? referencia = null)
        {
            if (valor <= 0) throw new ArgumentException("Valor deve ser positivo.");

            // Transaction garante que Saldo e Histórico sejam salvos juntos ou nenhum é salvo
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var c = await ObterOuCriarAsync(userId);
                c.Saldo += valor;

                _db.TransacoesCarteira.Add(new TransacaoCarteira
                {
                    CarteiraUsuarioId = c.Id,
                    Valor = valor,
                    Tipo = TipoTransacaoCarteira.Deposito,
                    Referencia = referencia ?? "Depósito",
                    SaldoApos = c.Saldo,
                    Data = DateTime.Now
                });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw; // Relança para o Controller tratar
            }
        }

        public async Task DebitarParaPedidoAsync(string userId, decimal valor, int pedidoId)
        {
            if (valor <= 0) throw new ArgumentException("Valor deve ser positivo.");

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var c = await ObterOuCriarAsync(userId);
                if (c.Saldo < valor) throw new InvalidOperationException("Saldo insuficiente.");

                c.Saldo -= valor;

                _db.TransacoesCarteira.Add(new TransacaoCarteira
                {
                    CarteiraUsuarioId = c.Id,
                    Valor = -valor,
                    Tipo = TipoTransacaoCarteira.DebitoPagamentoPedido,
                    PedidoId = pedidoId,
                    Referencia = $"Pagamento pedido #{pedidoId}",
                    SaldoApos = c.Saldo,
                    Data = DateTime.Now
                });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> JaEstornadoAsync(int pedidoId)
        {
            return await _db.TransacoesCarteira
                .AnyAsync(t => t.PedidoId == pedidoId && t.Tipo == TipoTransacaoCarteira.EstornoCancelamento);
        }

        public async Task EstornarPedidoAsync(string userId, decimal valor, int pedidoId)
        {
            if (valor <= 0) return;
            if (await JaEstornadoAsync(pedidoId)) return;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var c = await ObterOuCriarAsync(userId);
                c.Saldo += valor;

                _db.TransacoesCarteira.Add(new TransacaoCarteira
                {
                    CarteiraUsuarioId = c.Id,
                    Valor = valor,
                    Tipo = TipoTransacaoCarteira.EstornoCancelamento,
                    PedidoId = pedidoId,
                    Referencia = $"Estorno pedido #{pedidoId}",
                    SaldoApos = c.Saldo,
                    Data = DateTime.Now
                });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}