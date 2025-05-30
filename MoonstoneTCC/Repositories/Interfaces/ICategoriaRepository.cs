using MoonstoneTCC.Models;

namespace MoonstoneTCC.Repositories.Interfaces
{
    public interface ICategoriaRepository
    {
        IEnumerable<Categoria> Categorias { get; }
    }
}
