using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminUsuariosPedidosController : Controller
    {
        private readonly AppDbContext _context;

        public AdminUsuariosPedidosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult VerPedidos(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound("Email do usuário não informado.");
            }

            var pedidos = _context.Pedidos
     .AsNoTracking()
     .Include(p => p.PedidoItens)
     .ThenInclude(i => i.Jogo)
     .Where(p => p.Email == email)
     .OrderByDescending(p => p.PedidoEnviado)
     .ToList();


            ViewBag.EmailUsuario = email;
            return View(pedidos);
        }
    }
}

