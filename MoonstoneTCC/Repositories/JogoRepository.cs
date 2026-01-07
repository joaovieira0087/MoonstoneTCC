using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;

namespace MoonstoneTCC.Repositories
{
    public class JogoRepository : IJogoRepository
    {
        private readonly AppDbContext _context;
        public JogoRepository(AppDbContext contexto)
        {
            _context = contexto;
        }

        public IEnumerable<Jogo> Jogos => _context.Jogos.Include(c => c.Categoria);

        public IEnumerable<Jogo> JogosPreferidos => _context.Jogos
        .Where(j => j.IsJogoPreferido)
        .Include(j => j.Categoria)
        .Include(j => j.Comentarios); // ← Necessário para AvaliacaoMedia


        public Jogo GetJogoById(int jogoId)
        {
            return _context.Jogos.FirstOrDefault(j => j.JogoId == jogoId);
        }

        public IEnumerable<Jogo> GetJogosRecomendados(int jogoId, int quantidade)
        {
            var jogoAtual = _context.Jogos.FirstOrDefault(j => j.JogoId == jogoId);
            if (jogoAtual == null) return Enumerable.Empty<Jogo>();

            // Jogos na mesma categoria, excluindo o atual
            return _context.Jogos
                .Where(j => j.CategoriaId == jogoAtual.CategoriaId && j.JogoId != jogoId)
                .OrderBy(_ => Guid.NewGuid()) // Aleatoriza
                .Take(quantidade)
                .Include(j => j.Categoria)
                .ToList();
        }

        public IEnumerable<Jogo> GetJogosMaisComprados(int quantidade)
        {
            return _context.PedidoDetalhes
                .GroupBy(p => p.JogoId)
                .Select(g => new
                {
                    JogoId = g.Key,
                    QuantidadeTotal = g.Sum(x => x.Quantidade)
                })
                .OrderByDescending(x => x.QuantidadeTotal)
                .Take(quantidade)
                .Join(_context.Jogos.Include(j => j.Categoria).Include(j => j.Comentarios),
                      comp => comp.JogoId,
                      jogo => jogo.JogoId,
                      (comp, jogo) => jogo)
                .ToList();
        }

        public IEnumerable<Jogo> GetJogosMaisCompradosPorCategoria(int categoriaId, int quantidade)
        {
            return _context.PedidoDetalhes
                .Where(p => p.Jogo.CategoriaId == categoriaId)
                .GroupBy(p => p.JogoId)
                .Select(g => new
                {
                    JogoId = g.Key,
                    QuantidadeTotal = g.Sum(x => x.Quantidade)
                })
                .OrderByDescending(x => x.QuantidadeTotal)
                .Take(quantidade)
                .Join(_context.Jogos.Include(j => j.Categoria),
                      comp => comp.JogoId,
                      jogo => jogo.JogoId,
                      (comp, jogo) => jogo)
                .ToList();
        }

        public int GetClassificacaoRanking(int jogoId)
        {
            var ranking = _context.PedidoDetalhes
                .GroupBy(p => p.JogoId)
                .Select(g => new
                {
                    JogoId = g.Key,
                    Total = g.Sum(x => x.Quantidade)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            var posicao = ranking.FindIndex(r => r.JogoId == jogoId);
            return posicao >= 0 ? posicao + 1 : 0; // +1 pois index começa em 0
        }

        public List<Jogo> GetJogosMaisBuscados(int quantidade)
        {
            return _context.HistoricoVisualizacoes
                .GroupBy(h => h.JogoId)
                .OrderByDescending(g => g.Count())
                .Take(quantidade)
                .Select(g => g.Key)
                .Join(_context.Jogos, id => id, jogo => jogo.JogoId, (id, jogo) => jogo)
                .ToList();
        }




    }
}
