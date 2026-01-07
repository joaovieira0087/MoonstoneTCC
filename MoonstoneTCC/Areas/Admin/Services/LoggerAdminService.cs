using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MoonstoneTCC.Services
{
    public class LoggerAdminService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public LoggerAdminService(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public async Task RegistrarAcaoAsync(string descricao)
        {
            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var log = new AcaoAdmin
                {
                    UsuarioId = userId,
                    Acao = descricao,
                    DataHora = DateTime.Now
                };

                _context.AcoesAdmin.Add(log);
                await _context.SaveChangesAsync();
            }
        }
    }
}
