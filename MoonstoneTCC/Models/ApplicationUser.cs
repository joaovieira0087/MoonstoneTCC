using Microsoft.AspNetCore.Identity;

namespace MoonstoneTCC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
    }
}