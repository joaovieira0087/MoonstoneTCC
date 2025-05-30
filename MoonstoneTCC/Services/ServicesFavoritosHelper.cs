using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace MoonstoneTCC.Services
{
    public static class FavoritosHelper
    {
        private const string FavoritosKey = "Favoritos";

        public static List<int> GetFavoritos(ISession session)
        {
            var favoritos = session.GetString(FavoritosKey);
            return favoritos == null ? new List<int>() : JsonSerializer.Deserialize<List<int>>(favoritos);
        }

        public static void AdicionarFavorito(ISession session, int jogoId)
        {
            var favoritos = GetFavoritos(session);
            if (!favoritos.Contains(jogoId))
            {
                favoritos.Add(jogoId);
                session.SetString(FavoritosKey, JsonSerializer.Serialize(favoritos));
            }
        }

        public static void RemoverFavorito(ISession session, int jogoId)
        {
            var favoritos = GetFavoritos(session);
            if (favoritos.Contains(jogoId))
            {
                favoritos.Remove(jogoId);
                session.SetString(FavoritosKey, JsonSerializer.Serialize(favoritos));
            }
        }
    }
}
