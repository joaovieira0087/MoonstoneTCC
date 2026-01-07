using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.EntityFrameworkCore;


[Authorize]
public class CartaoController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CartaoController(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var cartoes = await _context.CartoesCredito
            .Where(c => c.UserId == user.Id)
            .ToListAsync();

        return View(cartoes);
    }

    public IActionResult Adicionar() => View();

    [HttpPost]
    public async Task<IActionResult> Adicionar(CartaoCredito model, string numeroCompleto)
    {
        var user = await _userManager.GetUserAsync(User);

        model.UserId = user.Id;
        model.NumeroParcial = numeroCompleto[^4..]; // salva só os últimos 4
        model.CartaoPadrao = !_context.CartoesCredito.Any(c => c.UserId == user.Id);

        _context.CartoesCredito.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Excluir(int id)
    {
        var cartao = await _context.CartoesCredito.FindAsync(id);
        if (cartao == null) return NotFound();
        return View(cartao);
    }

    [HttpPost, ActionName("Excluir")]
    public async Task<IActionResult> ConfirmarExclusao(int id)
    {
        var cartao = await _context.CartoesCredito.FindAsync(id);
        if (cartao == null) return NotFound();

        _context.CartoesCredito.Remove(cartao);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> DefinirComoPadrao(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var cartoes = await _context.CartoesCredito.Where(c => c.UserId == user.Id).ToListAsync();

        foreach (var c in cartoes) c.CartaoPadrao = false;
        var cartaoPadrao = cartoes.FirstOrDefault(c => c.Id == id);
        if (cartaoPadrao != null) cartaoPadrao.CartaoPadrao = true;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
