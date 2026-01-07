using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using MoonstoneTCC.ViewModels;

public class FavoritoRepository : IFavoritoRepository
{
    private readonly AppDbContext _context;

    public FavoritoRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Adicionar(string usuarioId, int jogoId)
    {
        if (!JaFavoritou(usuarioId, jogoId))
        {
            var favorito = new Favorito
            {
                UsuarioId = usuarioId,
                JogoId = jogoId
            };

            _context.Favoritos.Add(favorito);
            _context.SaveChanges();
        }
    }

    public void Remover(string usuarioId, int jogoId)
    {
        var favorito = _context.Favoritos
            .FirstOrDefault(f => f.UsuarioId == usuarioId && f.JogoId == jogoId);

        if (favorito != null)
        {
            _context.Favoritos.Remove(favorito);
            _context.SaveChanges();
        }
    }

    public bool JaFavoritou(string usuarioId, int jogoId)
    {
        return _context.Favoritos.Any(f => f.UsuarioId == usuarioId && f.JogoId == jogoId);
    }

    public IEnumerable<Jogo> GetFavoritosDoUsuario(string usuarioId)
    {
        return _context.Favoritos
            .Where(f => f.UsuarioId == usuarioId)
            .Include(f => f.Jogo)
                .ThenInclude(j => j.Categoria)
            .Select(f => f.Jogo)
            .ToList();
    }

    // Método usado no Controller para verificação clara
    public bool JaAdicionado(string usuarioId, int jogoId)
    {
        return JaFavoritou(usuarioId, jogoId);
    }

    public Favorito ObterFavorito(string userId, int jogoId)
    {
        return _context.Favoritos
            .FirstOrDefault(f => f.UsuarioId == userId && f.JogoId == jogoId);
    }

    public IEnumerable<Favorito> GetFavoritosPublicosDoUsuario(string usuarioId)
    {
        return _context.Favoritos
            .Where(f => f.UsuarioId == usuarioId && f.EPublico)
            .Include(f => f.Jogo)
            .ToList();
    }

    public IEnumerable<FavoritoViewModel> GetFavoritosComDetalhesDoUsuario(string usuarioId)
    {
        return _context.Favoritos
            .Where(f => f.UsuarioId == usuarioId)
            .Include(f => f.Jogo)
                .ThenInclude(j => j.Categoria)
            .Select(f => new FavoritoViewModel
            {
                Jogo = f.Jogo,
                EPublico = f.EPublico,
                TagFavorito = f.TagFavorito          
            })
            .ToList();
    }



    public void Salvar()
    {
        _context.SaveChanges();
    }
}
