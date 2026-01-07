using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class EnderecoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EnderecoController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var enderecos = await _context.EnderecosEntrega
                .Where(e => e.UserId == user.Id)
                .ToListAsync();

            return View(enderecos);
        }

        public IActionResult Adicionar() => View();

        [HttpPost]
        public async Task<IActionResult> Adicionar(EnderecoEntrega endereco)
        {
            var user = await _userManager.GetUserAsync(User);
            endereco.UserId = user.Id;
            endereco.Email = user.Email;


            // Primeiro endereço vira padrão
            bool temPadrao = await _context.EnderecosEntrega.AnyAsync(e => e.UserId == user.Id && e.EnderecoPadrao);
            endereco.EnderecoPadrao = !temPadrao;

            _context.EnderecosEntrega.Add(endereco);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var endereco = await _context.EnderecosEntrega.FindAsync(id);
            return View(endereco);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(EnderecoEntrega endereco)
        {
            _context.EnderecosEntrega.Update(endereco);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Excluir(int id)
        {
            var endereco = await _context.EnderecosEntrega.FindAsync(id);
            if (endereco == null)
                return NotFound();

            return View(endereco);
        }

        [HttpPost, ActionName("Excluir")]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            var endereco = await _context.EnderecosEntrega.FindAsync(id);
            if (endereco == null)
                return NotFound();

            _context.EnderecosEntrega.Remove(endereco);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Visualizar(int id)
        {
            var endereco = await _context.EnderecosEntrega.FindAsync(id);
            if (endereco == null)
            {
                return NotFound();
            }

            return View(endereco);
        }



        public async Task<IActionResult> DefinirComoPadrao(int id)
        {
            var endereco = await _context.EnderecosEntrega.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            var enderecos = await _context.EnderecosEntrega.Where(e => e.UserId == user.Id).ToListAsync();
            foreach (var e in enderecos)
                e.EnderecoPadrao = false;

            endereco.EnderecoPadrao = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
