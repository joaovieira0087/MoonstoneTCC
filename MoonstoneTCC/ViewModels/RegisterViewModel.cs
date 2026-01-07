using System.ComponentModel.DataAnnotations;

namespace MoonstoneTCC.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Informe o nome de usuário")]
        [Display(Name = "Nome de Usuário")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Informe o email")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe a senha")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirme a senha")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmPassword { get; set; }
    }
}
