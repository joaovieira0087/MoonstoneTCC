using MoonstoneTCC.Models;

namespace MoonstoneTCC.Repositories.Interfaces
{
    public interface IAcessorioRepository
    {
        IEnumerable<Acessorio> Acessorios { get; }
        IEnumerable<Acessorio> GetAcessoriosPromocionais();
        Acessorio GetAcessorioById(int acessorioId);
    }
}

