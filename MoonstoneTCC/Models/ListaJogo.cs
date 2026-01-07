using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    public class ListaJogo
    {
        [Key]
        public int ListaJogoId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nome { get; set; }

        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        public ICollection<ItemListaJogo> Jogos { get; set; }

        // PERFIL PUBLICO 

        public bool EPublica { get; set; } = true;

    }
}
