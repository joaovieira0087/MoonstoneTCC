using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.ViewModels;

[Authorize]
public class PerfilController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _context;

    public PerfilController(UserManager<IdentityUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Visualizar()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var totalSeguidores = await _context.SeguidoresUsuarios.CountAsync(s => s.SeguidoId == user.Id);
        var totalSeguindo = await _context.SeguidoresUsuarios.CountAsync(s => s.SeguidorId == user.Id);

        ViewBag.TotalSeguidores = totalSeguidores;
        ViewBag.TotalSeguindo = totalSeguindo;


        var ultimoPedido = await _context.Pedidos
        .Where(p => p.UserId == user.Id)
        .OrderByDescending(p => p.PedidoEnviado)
        .Include(p => p.PedidoItens)
            .ThenInclude(i => i.Jogo)
        .FirstOrDefaultAsync();

        var totalPedidos = await _context.Pedidos.CountAsync(p => p.UserId == user.Id);
        var interesses = await _context.InteressesUsuarios
            .Where(i => i.UsuarioId == user.Id)
            .Select(i => i.Interesse)
            .ToListAsync();

        ViewBag.InteressesUsuario = interesses;

        var model = new UserProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Nome = ultimoPedido?.Nome,
            Sobrenome = ultimoPedido?.Sobrenome,
            Endereco1 = ultimoPedido?.Endereco1,
            Endereco2 = ultimoPedido?.Endereco2,
            Cidade = ultimoPedido?.Cidade,
            Estado = ultimoPedido?.Estado,
            Cep = ultimoPedido?.Cep,
            Telefone = ultimoPedido?.Telefone,
            DataUltimoPedido = ultimoPedido?.PedidoEnviado ?? DateTime.MinValue,
            TotalPedidos = totalPedidos,
            UltimoPedido = ultimoPedido
        };


        return View("Visualizar", model);



        
    }
    // PERFIL PUBLICPO
    [AllowAnonymous]
    [HttpGet("/Usuario/Perfil/{id}")]
    public async Task<IActionResult> PerfilPublico(string id)
    {
        var totalSeguidores = await _context.SeguidoresUsuarios.CountAsync(s => s.SeguidoId == id);
        var totalSeguindo = await _context.SeguidoresUsuarios.CountAsync(s => s.SeguidorId == id);
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
            return NotFound();

        string usuarioAtualId = null;
        bool estaSeguindo = false;

        if (User.Identity.IsAuthenticated)
        {
            var usuarioAtual = await _userManager.GetUserAsync(User);
            usuarioAtualId = usuarioAtual?.Id;

            if (usuarioAtualId != null && usuarioAtualId != id)
            {
                estaSeguindo = await _context.SeguidoresUsuarios
                    .AnyAsync(s => s.SeguidorId == usuarioAtualId && s.SeguidoId == id);
            }
        }

        var interesses = await _context.InteressesUsuarios
            .Where(i => i.UsuarioId == id)
            .Select(i => i.Interesse)
            .ToListAsync();

        var comentarios = await _context.ComentariosJogo
            .Where(c => c.UsuarioId == id)
            .Include(c => c.Jogo)
            .OrderByDescending(c => c.Data)
            .ToListAsync();

        var listasPublicas = await _context.ListasJogos
            .Include(l => l.Jogos).ThenInclude(j => j.Jogo)
            .Where(l => l.UsuarioId == id && l.EPublica)
            .ToListAsync();

        var avaliacoes = await _context.AvaliacoesJogos
            .Include(a => a.Jogo)
            .Where(a => a.UsuarioId == id)
            .ToListAsync();

        var favoritosPublicos = await _context.Favoritos
                 .Where(f => f.UsuarioId == usuario.Id && f.EPublico)
                 .Include(f => f.Jogo)
                 .Select(f => f.Jogo)
                 .ToListAsync();


        var model = new PerfilPublicoViewModel
        {
            UsuarioId = id,
            NomeUsuario = usuario.UserName,
            Interesses = interesses,
            Comentarios = comentarios,
            ListasPublicas = listasPublicas,
            EstaSeguindo = estaSeguindo,
            TotalSeguidores = totalSeguidores,
            TotalSeguindo = totalSeguindo,
            FavoritosPublicos = favoritosPublicos ?? new List<Jogo>(),
            AvaliacoesLikes = avaliacoes.Where(a => a.Gostou == true).ToList(),
            AvaliacoesDislikes = avaliacoes.Where(a => a.Gostou == false).ToList()
        };


        return View("PerfilPublico", model);
    }

    public async Task<IActionResult> ExperienciaCompras()
    {
        var usuarioId = _userManager.GetUserId(User);

        var pedidos = await _context.Pedidos
            .Include(p => p.PedidoItens)
                .ThenInclude(pi => pi.Jogo)
            .Where(p => p.UserId == usuarioId)
            .ToListAsync();

        var jogos = pedidos
            .SelectMany(p => p.PedidoItens.Select(pi => pi.Jogo))
            .Distinct()
            .ToList();

        return View(jogos);
    }

}
