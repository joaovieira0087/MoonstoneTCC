using MoonstoneTCC.Models;
using MoonstoneTCC.ViewModels;

public interface IFavoritoRepository
{
    void Adicionar(string usuarioId, int jogoId);
    void Remover(string usuarioId, int jogoId);
    bool JaFavoritou(string usuarioId, int jogoId);
    IEnumerable<Jogo> GetFavoritosDoUsuario(string usuarioId);
    bool JaAdicionado(string usuarioId, int jogoId);
    Favorito ObterFavorito(string userId, int jogoId);
    IEnumerable<Favorito> GetFavoritosPublicosDoUsuario(string usuarioId);
    IEnumerable<FavoritoViewModel> GetFavoritosComDetalhesDoUsuario(string usuarioId);

    void Salvar();
}
