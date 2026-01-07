using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminAcessoriosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LoggerAdminService _logger;

        public AdminAcessoriosController(AppDbContext context, LoggerAdminService logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var acessorios = await _context.Acessorios.Include(a => a.Categoria).OrderBy(a => a.Nome).ToListAsync();
            return View(acessorios);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var acessorio = await _context.Acessorios.Include(a => a.Categoria).FirstOrDefaultAsync(a => a.AcessorioId == id);
            if (acessorio == null) return NotFound();

            return View(acessorio);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Acessorio acessorio)
        {
            Console.WriteLine("Entrou no POST de Create");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState inválido:");
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"Erro no campo {modelState.Key}: {error.ErrorMessage}");
                    }
                }

                ViewBag.Categorias = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", acessorio.CategoriaId);
                return View(acessorio);
            }

            _context.Add(acessorio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var acessorio = await _context.Acessorios.FindAsync(id);
            if (acessorio == null) return NotFound();

            ViewBag.Categorias = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", acessorio.CategoriaId);
            return View(acessorio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Acessorio acessorio)
        {
            if (id != acessorio.AcessorioId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(acessorio);
                    await _context.SaveChangesAsync();
                    await _logger.RegistrarAcaoAsync($"Editou o acessório: {acessorio.Nome}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcessorioExists(acessorio.AcessorioId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", acessorio.CategoriaId);
            return View(acessorio);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var acessorio = await _context.Acessorios.Include(a => a.Categoria).FirstOrDefaultAsync(a => a.AcessorioId == id);
            if (acessorio == null) return NotFound();

            return View(acessorio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var acessorio = await _context.Acessorios.FindAsync(id);
            if (acessorio != null)
            {
                _context.Acessorios.Remove(acessorio);
                await _context.SaveChangesAsync();
                await _logger.RegistrarAcaoAsync($"Excluiu o acessório: {acessorio.Nome}");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AcessorioExists(int id)
        {
            return _context.Acessorios.Any(a => a.AcessorioId == id);
        }
    }
}
