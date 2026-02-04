using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Services;

public interface IGamificacaoService
{
    Task AdicionarXPPorAvaliacaoAsync(string userId);
    Task<GamificacaoUsuario> ObterProgressoAsync(string userId);
}

public class GamificacaoService : IGamificacaoService
{
    private readonly AppDbContext _db;
    private readonly ICarteiraService _carteira;
    private const int XP_POR_AVALIACAO = 10;
    private const int XP_PARA_RECOMPENSA = 100;
    private const decimal VALOR_RECOMPENSA = 5.00m;

    public GamificacaoService(AppDbContext db, ICarteiraService carteira)
    {
        _db = db;
        _carteira = carteira;
    }

    public async Task<GamificacaoUsuario> ObterProgressoAsync(string userId)
    {
        var g = await _db.GamificacaoUsuarios.FirstOrDefaultAsync(x => x.UsuarioId == userId);
        if (g == null)
        {
            g = new GamificacaoUsuario { UsuarioId = userId };
            _db.GamificacaoUsuarios.Add(g);
            await _db.SaveChangesAsync();
        }
        return g;
    }

    public async Task AdicionarXPPorAvaliacaoAsync(string userId)
    {
        var g = await ObterProgressoAsync(userId);
        g.XPAtual += XP_POR_AVALIACAO;
        g.XPTotalAcumulado += XP_POR_AVALIACAO;
        g.TotalAvaliacoesRealizadas++;

        // Verifica se completou o ciclo de 100% (100 XP)
        if (g.XPAtual >= XP_PARA_RECOMPENSA)
        {
            g.XPAtual -= XP_PARA_RECOMPENSA; // Reinicia o contador de progresso
            g.ResgatesEfetuados++;

            // Credita o dinheiro na carteira existente
            await _carteira.DepositarAsync(userId, VALOR_RECOMPENSA, "Bônus por Avaliações de Jogos");
        }

        await _db.SaveChangesAsync();
    }
}