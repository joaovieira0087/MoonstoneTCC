using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    public class SeguidorUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SeguidorId { get; set; } // quem está seguindo

        [Required]
        public string SeguidoId { get; set; } // quem está sendo seguido

        [ForeignKey("SeguidorId")]
        public IdentityUser Seguidor { get; set; }

        [ForeignKey("SeguidoId")]
        public IdentityUser Seguido { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;


    }
}
