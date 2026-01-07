using System.ComponentModel.DataAnnotations;

namespace MoonstoneTCC.Models
{
    public class Desenvolvedora
    {
        public int DesenvolvedoraId { get; set; }

        [Required]
        public string Nome { get; set; }

        [StringLength(1500)]
        public string Descricao { get; set; }

        public string FotoPerfilUrl { get; set; }
        public string Curiosidades { get; set; }
        public string SlideImagens { get; set; } // URLs separadas por ";", por exemplo

        public List<Jogo> Jogos { get; set; }
        public List<SeguidorDesenvolvedora> Seguidores { get; set; }

    }
}

