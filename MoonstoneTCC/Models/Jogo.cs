using MoonstoneTCC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    [Table("Jogos")]
    public class Jogo
    {
        [Key]
        public int JogoId { get; set; }

        [Required(ErrorMessage = "O nome do jogo deve ser informado")]
        [Display(Name = "Nome do jogo")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O {0} deve ter no mínimo {1} e no máximo {2} caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição do jogo deve ser informada")]
        [Display(Name = "Descrição do jogo")]
        [MinLength(20, ErrorMessage = "Descrição deve ter no mínimo {1} caracteres")]
        [MaxLength(600, ErrorMessage = "Descrição pode exceder {1} caracteres")]
        public string DescricaoCurta { get; set; }

        [Required(ErrorMessage = "A descrição detalhada do jogo deve ser informada.")]
        [Display(Name = "Descrição detalhada do jogo")]
        [MinLength(20, ErrorMessage = "Descrição detalhada deve ter no mínimo {1} caracteres")]
        [MaxLength(1000, ErrorMessage = "Descrição detalhada pode exceder {1} caracteres")]
        public string DescricaoDetalhada { get; set; }

        [Required(ErrorMessage = "Informe o preço do jogo")]
        [Display(Name = "Preço")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(1, 999.99, ErrorMessage = "O preço deve estar entre 1 e 999,99")]
        public decimal Preco { get; set; }

        [Display(Name = "Caminho Imagem Normal")]
        [StringLength(1000, ErrorMessage = "O {0} deve ter no máximo {1} caracteres")]
        public string ImagemUrl { get; set; }

        [Display(Name = "Caminho Imagem Miniatura")]
        [StringLength(1000, ErrorMessage = "O {0} deve ter no máximo {1} caracteres")]
        public string ImagemThumbnailUrl { get; set; }

        [Display(Name = "Preferido?")]
        public bool IsJogoPreferido { get; set; }

        [Display(Name = "Estoque")]
        public bool EmEstoque { get; set; }

        [Required(ErrorMessage = "As plataformas devem ser informadas")]
        [Display(Name = "Plataformas disponíveis")]
        [MinLength(3, ErrorMessage = "As plataformas devem ter no mínimo {1} caracteres")]
        [MaxLength(200, ErrorMessage = "As plataformas não podem exceder {1} caracteres")]
        public string Plataformas { get; set; }

        [Required(ErrorMessage = "O gênero do jogo deve ser informado")]
        [Display(Name = "Gênero do jogo")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
        public string Genero { get; set; }

        [Required(ErrorMessage = "A classificação etária deve ser informada")]
        [Display(Name = "Classificação etária (idade mínima)")]
        [Range(0, 99, ErrorMessage = "A classificação etária deve ser um valor entre {1} e {2}")]
        public string ClassificacaoEtaria { get; set; }

        [Display(Name = "Imagem da classificação")]
        [Url(ErrorMessage = "O caminho da imagem deve ser válido")]
        public string ClassificacaoEtariaImagemUrl { get; set; }


        [Display(Name = "Informações extras")]
        [StringLength(1500, ErrorMessage = "As informações extras devem ter no máximo {1} caracteres")]
        public string InformacoesExtras { get; set; }

        [Display(Name = "Imagens adicional")]
        public string ImagensAdicionais { get; set; }

        [Display(Name = "Imagens adicional 2")]
        public string ImagensAdicionais2 { get; set; }

        [Display(Name = "Imagens adicional 3")]
        public string ImagensAdicionais3 { get; set; }

        [Display(Name = "Link do trailer no YouTube")]
        [Url(ErrorMessage = "O link deve ser uma URL válida")]
        [StringLength(1000, ErrorMessage = "O {0} deve ter no máximo {1} caracteres")]
        public string TrailerYoutubeUrl { get; set; }


        public int CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }


    }
}
