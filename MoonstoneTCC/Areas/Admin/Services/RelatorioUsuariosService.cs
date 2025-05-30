using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MoonstoneTCC.Areas.Admin.Services
{
    public class RelatorioUsuariosService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public RelatorioUsuariosService(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IEnumerable<UsuarioRelatorioModel>> GetUsuariosAsync()
        {
            var usuarios = await _userManager.Users.ToListAsync();

            var lista = new List<UsuarioRelatorioModel>();

            foreach (var user in usuarios)
            {
                var pedidos = _context.Pedidos.Where(p => p.Email == user.Email).ToList();

                lista.Add(new UsuarioRelatorioModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    PedidosQuantidade = pedidos.Count,
                    DataCadastro = user.Id.Substring(0, 8) // ou qualquer lógica de data que você usar
                });
            }

            return lista;
        }
    }
}
