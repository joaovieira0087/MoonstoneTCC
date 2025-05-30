using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    public class PerguntaComunicado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ComunicadoId { get; set; }

        [ForeignKey("ComunicadoId")]
        public Comunicado Comunicado { get; set; }

        [Required]
        [StringLength(500)]
        public string TextoPergunta { get; set; }
    }
}
