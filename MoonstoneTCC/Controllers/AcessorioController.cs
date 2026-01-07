using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;

namespace MoonstoneTCC.Controllers
{
    public class AcessorioController : Controller
    {
        private readonly IAcessorioRepository _acessorioRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public AcessorioController(IAcessorioRepository acessorioRepository, UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _acessorioRepository = acessorioRepository;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult List(string categoria)
        {
            IEnumerable<Acessorio> acessorios;
            string categoriaAtual;

            if (string.IsNullOrEmpty(categoria))
            {
                acessorios = _acessorioRepository.Acessorios.OrderBy(a => a.AcessorioId);
                categoriaAtual = "Acessórios";
            }
            else
            {
                acessorios = _acessorioRepository.Acessorios
                    .Where(a => a.Categoria.CategoriaNome.Equals(categoria))
                    .OrderBy(a => a.Nome);
                categoriaAtual = categoria;
            }

            ViewData["CategoriaAtual"] = categoriaAtual;

            var viewModel = new AcessorioListViewModel
            {
                Acessorios = acessorios,
                CategoriaAtual = categoriaAtual
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int acessorioId)
        {
            var acessorio = await _context.Acessorios
                .Include(a => a.Categoria)
                .FirstOrDefaultAsync(a => a.AcessorioId == acessorioId);

            if (acessorio == null)
                return NotFound();

            return View(acessorio);
        }

        public ViewResult Search(string termo)
        {
            termo = termo?.Trim().ToLower();

            IEnumerable<Acessorio> acessorios;
            string categoriaAtual;

            if (string.IsNullOrEmpty(termo))
            {
                acessorios = _acessorioRepository.Acessorios.OrderBy(a => a.Nome);
                categoriaAtual = "Todos os Acessórios";
            }
            else
            {
                acessorios = _acessorioRepository.Acessorios
                    .Where(a => a.Nome.ToLower().Contains(termo)
                             || (a.DescricaoCurta != null && a.DescricaoCurta.ToLower().Contains(termo))
                             || (a.DescricaoDetalhada != null && a.DescricaoDetalhada.ToLower().Contains(termo))
                             || (a.Marca != null && a.Marca.ToLower().Contains(termo))
                             || (a.Modelo != null && a.Modelo.ToLower().Contains(termo))
                             || (a.Categoria != null && a.Categoria.CategoriaNome.ToLower().Contains(termo)))
                    .OrderBy(a => a.Nome);

                categoriaAtual = acessorios.Any() ? $"Resultados para '{termo}'" : "Nenhum acessório encontrado";
            }

            return View("~/Views/Acessorio/List.cshtml", new AcessorioListViewModel
            {
                Acessorios = acessorios,
                CategoriaAtual = categoriaAtual
            });
        }

        public IActionResult Promocoes()
        {
            var acessoriosEmPromocao = _context.Acessorios
                .Where(a => a.PrecoPromocional != null)
                .ToList();

            return View(acessoriosEmPromocao);
        }
    }
}
