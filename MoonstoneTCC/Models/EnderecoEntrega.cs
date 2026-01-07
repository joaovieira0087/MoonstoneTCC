using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    public class EnderecoEntrega
    {
        public int EnderecoEntregaId { get; set; }
      
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser Usuario { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required]
        [StringLength(50)]
        public string Sobrenome { get; set; }

        [Required]
        [StringLength(100)]
        public string Endereco1 { get; set; }

        [StringLength(100)]
        public string Endereco2 { get; set; }

        [Required]
        [StringLength(50)]
        public string Cidade { get; set; }

        [Required]
        [StringLength(10)]
        public string Estado { get; set; }

        [Required]
        [StringLength(10)]
        public string Cep { get; set; }

        [Required]
        [StringLength(25)]
        public string Telefone { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }



        public bool EnderecoPadrao { get; set; }
    }
}
