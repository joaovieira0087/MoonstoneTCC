// Models/ViewModels/DepositoViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace MoonstoneTCC.Models.ViewModels
{
    public class DepositoViewModel
    {
        [Display(Name = "Valor do Depósito")]
        [Required(ErrorMessage = "Por favor, informe um valor.")]
        [Range(1.00, 10000.00, ErrorMessage = "O valor deve ser entre R$ 1,00 e R$ 10.000,00.")]
        public decimal? Valor { get; set; } // Nullable para não validar 0 como padrão errôneo antes do input
    }
}