using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.ViewModels;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class MeusPedidosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public MeusPedidosController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Lista todos os pedidos do usuário logado
        // 10 por página
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var usuario = await _userManager.GetUserAsync(User);

            // Base query (não materializa ainda!)
            var baseQuery = _context.Pedidos
                .Where(p => p.UserId == usuario.Id);

            var totalCount = await baseQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var pedidos = await baseQuery
                .Include(p => p.PedidoItens).ThenInclude(i => i.Jogo)
                .Include(p => p.PedidoItens).ThenInclude(i => i.Acessorio)
                .Include(p => p.Cancelamento)
                .OrderByDescending(p => p.PedidoEnviado)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Jogos pendentes de avaliação (continua considerando TODOS os pedidos, não apenas a página)
            var jogosComprados = await _context.PedidoDetalhes
                .Include(p => p.Pedido)
                .Where(p => p.Pedido.UserId == usuario.Id && p.JogoId != null)
                .Select(p => p.JogoId!.Value)
                .Distinct()
                .ToListAsync();

            var jogosAvaliados = await _context.ComentariosJogo
                .Where(c => c.UsuarioId == usuario.Id)
                .Select(c => c.JogoId)
                .ToListAsync();

            ViewBag.JogosPendentes = jogosComprados.Except(jogosAvaliados).Count();

            var vm = new MeusPedidosIndexViewModel
            {
                Pedidos = pedidos,
                Page = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View("Index", vm);
        }



    // Mostra os detalhes de um pedido específico
    public async Task<IActionResult> Detalhes(int id)
        {
            var usuario = await _userManager.GetUserAsync(User);

            var pedido = await _context.Pedidos
            .Include(p => p.PedidoItens)
                .ThenInclude(pi => pi.Jogo)
            .Include(p => p.PedidoItens)
                .ThenInclude(pi => pi.Acessorio)
            .FirstOrDefaultAsync(p => p.PedidoId == id && p.UserId == usuario.Id);


            if (pedido == null)
                return NotFound();

            return View("Detalhes", pedido); // Views/MeusPedidos/Detalhes.cshtml
        }

        public async Task<IActionResult> Avaliar()
        {
            var usuario = await _userManager.GetUserAsync(User);

            // Buscar todos os jogos comprados pelo usuário
            var jogosComprados = await _context.PedidoDetalhes
                .Include(p => p.Jogo)
                .Include(p => p.Pedido)
                .Where(p => p.Pedido.UserId == usuario.Id && p.Jogo != null) // Evita nulos
                .Select(p => p.Jogo)
                .Distinct() // Para evitar jogos repetidos
                .ToListAsync();

            // Buscar os IDs dos jogos já avaliados por esse usuário
            var jogosAvaliados = await _context.ComentariosJogo
                .Where(c => c.UsuarioId == usuario.Id)
                .Select(c => c.JogoId)
                .ToListAsync();

            // Retornar os jogos que ainda não foram avaliados
            var jogosParaAvaliar = jogosComprados
                .Where(j => !jogosAvaliados.Contains(j.JogoId))
                .ToList();

            return View(jogosParaAvaliar); // View espera List<Jogo>
        }



    }
}
