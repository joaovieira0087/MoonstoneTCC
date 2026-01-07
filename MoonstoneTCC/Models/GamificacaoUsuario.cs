// Models/GamificacaoUsuario.cs
using System.ComponentModel.DataAnnotations;

namespace MoonstoneTCC.Models
{
    public class GamificacaoUsuario
    {
        [Key]
        public int Id { get; set; }
        public string UsuarioId { get; set; } = default!;
        public int XPAtual { get; set; }
        public int XPTotalAcumulado { get; set; }
        public int TotalAvaliacoesRealizadas { get; set; }

        // Quantas vezes o usuário já resgatou o prêmio de dinheiro
        public int ResgatesEfetuados { get; set; }
    }
}