using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MoonstoneTCC.Models
{
    public class SeguidorDesenvolvedora
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }

        public int DesenvolvedoraId { get; set; }
        public Desenvolvedora Desenvolvedora { get; set; }
    }
}
