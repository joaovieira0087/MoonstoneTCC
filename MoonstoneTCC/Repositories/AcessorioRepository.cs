using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MoonstoneTCC.Repositories
{
    public class AcessorioRepository : IAcessorioRepository
    {
        private readonly AppDbContext _context;

        public AcessorioRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Acessorio> Acessorios => _context.Acessorios.Include(a => a.Categoria);

        public Acessorio GetAcessorioById(int acessorioId)
        {
            return _context.Acessorios
                .Include(a => a.Categoria)
                .FirstOrDefault(a => a.AcessorioId == acessorioId);
        }

        public IEnumerable<Acessorio> GetAcessoriosPromocionais()
        {
            return _context.Acessorios
                .Where(a => a.PrecoPromocional != null)
                .Include(a => a.Categoria)
                .ToList();
        }
    }
}
