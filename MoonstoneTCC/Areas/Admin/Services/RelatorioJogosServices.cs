using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.EntityFrameworkCore;

namespace MoonstoneTCC.Areas.Admin.Servicos;

public class RelatorioJogosService
{
    private readonly AppDbContext _context;

    public RelatorioJogosService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Jogo>> GetJogosReport()
    {
        var jogos = await _context.Jogos.ToListAsync();

        if (jogos is null)
            return default(IEnumerable<Jogo>);

        return jogos;
    }

    public async Task<IEnumerable<Categoria>> GetCategoriasReport()
    {
        var categorias = await _context.Categorias.ToListAsync();

        if (categorias is null)
            return default(IEnumerable<Categoria>);

        return categorias;
    }
}
