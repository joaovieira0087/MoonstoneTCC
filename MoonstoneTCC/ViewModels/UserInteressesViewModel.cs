using System.Collections.Generic;

namespace MoonstoneTCC.ViewModels
{
    public class UserInteressesViewModel
    {
        public List<string> InteressesSelecionados { get; set; } = new List<string>();
        public List<string> TodosOsInteresses { get; set; } = new List<string>();
    }
}