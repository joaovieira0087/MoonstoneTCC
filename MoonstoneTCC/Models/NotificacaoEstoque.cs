using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    public class NotificacaoEstoque
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }

        public int JogoId { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public bool Lida { get; set; } = false;

        public string? Mensagem { get; set; }

        // Relacionamentos
        public Jogo Jogo { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }
    }

}
