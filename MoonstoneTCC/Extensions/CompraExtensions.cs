using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;

namespace MoonstoneTCC.Extensions
{
    public static class CompraExtensions
    {
        /// Verifica se um e-mail já comprou um jogo específico.
        /// Troque p.Email para o nome real da propriedade de e-mail da sua entidade Pedido
        public static async Task<bool> UsuarioComprouJogoPorEmailAsync(
            this AppDbContext ctx, string email, int jogoId)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await ctx.Pedidos
                .Where(p => p.Email == email) // <-- AJUSTE se seu campo é UsuarioEmail/EmailCliente/etc.
                .AnyAsync(p => ctx.PedidoDetalhes
                    .Any(pd => pd.PedidoId == p.PedidoId && pd.JogoId == jogoId));
        }
    }
}


