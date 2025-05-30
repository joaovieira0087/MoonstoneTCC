using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Context
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {         
        }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<CarrinhoCompraItem> CarrinhoCompraItens { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalhe> PedidoDetalhes { get; set; }

        public DbSet<PerguntaUsuario> PerguntasUsuarios { get; set; }
        public DbSet<Comunicado> Comunicados { get; set; }
        public DbSet<RespostaUsuario> RespostasUsuarios { get; set; }
        public DbSet<PerguntaComunicado> PerguntasComunicados { get; set; }
    }
}
