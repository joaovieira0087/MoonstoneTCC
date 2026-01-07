using Microsoft.AspNetCore.Identity;
using MoonstoneTCC.Models;
using System.ComponentModel.DataAnnotations.Schema;
namespace MoonstoneTCC.Models
{
    public class AvisoEstoque
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        public int? JogoId { get; set; }
        public Jogo Jogo { get; set; }

        public int? AcessorioId { get; set; }
        public Acessorio Acessorio { get; set; }

        public bool Avisado { get; set; }
    }
}
