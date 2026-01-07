using Microsoft.AspNetCore.Identity;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class ResultadoBuscaViewModel
    {
        public IEnumerable<Jogo> Jogos { get; set; }
        public IEnumerable<Desenvolvedora> Desenvolvedoras { get; set; }
        public IEnumerable<IdentityUser> Usuarios { get; set; }
        public string TermoBuscado { get; set; }
    }

}
