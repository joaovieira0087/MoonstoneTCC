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

        public IEnumerable<Jogo> JogosPreferidos => _context.Jogos.Where(j => j.IsJogoPreferido).Include(j => j.Categoria);

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

    }
}
