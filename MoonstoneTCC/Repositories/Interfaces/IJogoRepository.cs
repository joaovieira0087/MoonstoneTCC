using MoonstoneTCC.Models;
using System.Collections.Generic;

namespace MoonstoneTCC.Repositories.Interfaces
{
    public interface IJogoRepository
    {
        IEnumerable<Jogo> Jogos { get; }

        IEnumerable<Jogo> JogosPreferidos { get; }

        Jogo GetJogoById(int jogoId);

        // Novo método para obter jogos recomendados
        IEnumerable<Jogo> GetJogosRecomendados(int jogoId, int quantidade);
    }
}
