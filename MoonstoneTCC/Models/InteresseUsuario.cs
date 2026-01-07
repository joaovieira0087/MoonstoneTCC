using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    [Table("InteressesUsuario")]
    public class InteresseUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Interesse { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }
    }
}
