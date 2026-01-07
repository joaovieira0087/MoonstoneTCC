using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    public class ItemListaJogo
    {
        [Key]
        public int Id { get; set; }

        public int ListaJogoId { get; set; }
        [ForeignKey("ListaJogoId")]
        public ListaJogo ListaJogo { get; set; }

        public int JogoId { get; set; }
        [ForeignKey("JogoId")]
        public Jogo Jogo { get; set; }
    }
}
