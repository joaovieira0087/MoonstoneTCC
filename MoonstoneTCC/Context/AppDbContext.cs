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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Impede deletar um usuário e apagar em cascata os seguidores relacionados
            modelBuilder.Entity<SeguidorUsuario>()
                .HasOne(s => s.Seguidor)
                .WithMany()
                .HasForeignKey(s => s.SeguidorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SeguidorUsuario>()
                .HasOne(s => s.Seguido)
                .WithMany()
                .HasForeignKey(s => s.SeguidoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Evita ciclo de deleção automática entre PerguntaVaga e RespostaCandidatura
            modelBuilder.Entity<RespostaCandidatura>()
                .HasOne(r => r.Pergunta)
                .WithMany()
                .HasForeignKey(r => r.PerguntaVagaId)
                .OnDelete(DeleteBehavior.Restrict); // ou .NoAction()

            modelBuilder.Entity<Candidatura>()
            .HasOne(c => c.EntrevistaAgendada)
            .WithOne(e => e.Candidatura)
            .HasForeignKey<EntrevistaAgendada>(e => e.CandidaturaId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarteiraUsuario>()
            .HasIndex(c => c.UserId)
            .IsUnique();

            modelBuilder.Entity<TransacaoCarteira>()
                .HasOne(t => t.Carteira)
                .WithMany(c => c.Transacoes)
                .HasForeignKey(t => t.CarteiraUsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
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
        public DbSet<AcaoAdmin> AcoesAdmin { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }
        public DbSet<ComentarioJogo> ComentariosJogo { get; set; }
        public DbSet<ComentarioCurtida> ComentarioCurtidas { get; set; }
        public DbSet<ListaJogo> ListasJogos { get; set; }
        public DbSet<ItemListaJogo> ItensListaJogos { get; set; }
        public DbSet<HistoricoVisualizacao> HistoricoVisualizacoes { get; set; }
        public DbSet<EnderecoEntrega> EnderecosEntrega { get; set; }
        public DbSet<InteresseUsuario> InteressesUsuarios { get; set; }
        public DbSet<CartaoCredito> CartoesCredito { get; set; }
        public DbSet<SeguidorUsuario> SeguidoresUsuarios { get; set; }
        public DbSet<Desenvolvedora> Desenvolvedoras { get; set; }
        public DbSet<SeguidorDesenvolvedora> SeguidoresDesenvolvedoras { get; set; }
        public DbSet<AvaliacaoJogo> AvaliacoesJogos { get; set; }
        public DbSet<AvisoEstoque> AvisosEstoque { get; set; }
        public DbSet<NotificacaoEstoque> NotificacoesEstoque { get; set; }
        public DbSet<NotificacaoPedido> NotificacoesPedido { get; set; }
        public DbSet<ExclusaoConta> ExclusoesConta { get; set; }





        // vaga de entrego 
        public DbSet<VagaEmprego> VagasEmprego { get; set; }
        public DbSet<PerguntaVaga> PerguntasVaga { get; set; }
        public DbSet<Candidatura> Candidaturas { get; set; }
        public DbSet<RespostaCandidatura> RespostasCandidatura { get; set; }
        public DbSet<HistoricoStatusCandidatura> HistoricoStatusCandidaturas { get; set; }
        public DbSet<EtapaProcessoSeletivo> EtapasProcessoSeletivo { get; set; }
        public DbSet<EntrevistaAgendada> EntrevistasAgendadas { get; set; }
        public DbSet<MensagemCandidatura> MensagensCandidatura { get; set; }

        // ajuda com pedido fixo 
        public DbSet<AjudaPedido> AjudasPedidos { get; set; }
        public DbSet<CancelamentoPedido> CancelamentosPedidos { get; set; }


        //  ACESSORIOS 
        public DbSet<Acessorio> Acessorios { get; set; }

        // cateira propria 
        public DbSet<CarteiraUsuario> Carteiras { get; set; }
        public DbSet<TransacaoCarteira> TransacoesCarteira { get; set; }




    }
}
