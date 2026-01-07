using MoonstoneTCC.Models;
using System.ComponentModel.DataAnnotations;

namespace MoonstoneTCC.ViewModels
{
    public class UserProfileViewModel
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Use apenas letras, números e underline.")]
        public string UserName { get; set; }

        public string Email { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Endereco1 { get; set; }
        public string Endereco2 { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
        public string Telefone { get; set; }

        public DateTime DataUltimoPedido { get; set; }
        public int TotalPedidos { get; set; }

        public Pedido? UltimoPedido { get; set; }

    }
}
