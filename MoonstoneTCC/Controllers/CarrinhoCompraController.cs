using System.Collections.Generic;    // Para Dictionary<string,int>
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.ViewModels;

namespace MoonstoneTCC.Controllers
{
    public class CarrinhoCompraController : Controller
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public CarrinhoCompraController(IJogoRepository jogoRepository,
                                        CarrinhoCompra carrinhoCompra)
        {
            _jogoRepository = jogoRepository;
            _carrinhoCompra = carrinhoCompra;
        }

        // GET: /CarrinhoCompra
        [HttpGet]
        public IActionResult Index()
        {
            var vm = CriarViewModel();
            return View(vm);
        }

        // POST: /CarrinhoCompra (aplica cupom)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string codigoCupom)
        {
            var vm = CriarViewModel();

            // Defina aqui seus cupons válidos e suas porcentagens
            var cupons = new Dictionary<string, int>
            {
                { "PROMO10", 10 },
                { "PROMO20", 20 }
            };

            if (!string.IsNullOrWhiteSpace(codigoCupom)
                && cupons.TryGetValue(codigoCupom.ToUpper(), out int pct))
            {
                vm.CodigoCupom = codigoCupom.ToUpper();
                vm.Desconto = vm.CarrinhoCompraTotal * pct / 100m;
                vm.TotalComDesconto = vm.CarrinhoCompraTotal - vm.Desconto;
            }

            return View(vm);
        }

        [Authorize]
        public IActionResult AdicionarItemNoCarrinhoCompra(int jogoId)
        {
            var jogoSelecionado = _jogoRepository.Jogos
                .FirstOrDefault(p => p.JogoId == jogoId);

            if (jogoSelecionado != null)
                _carrinhoCompra.AdicionarAoCarrinho(jogoSelecionado);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult RemoverItemDoCarrinhoCompra(int jogoId)
        {
            var jogoSelecionado = _jogoRepository.Jogos
                .FirstOrDefault(p => p.JogoId == jogoId);

            if (jogoSelecionado != null)
                _carrinhoCompra.RemoverDoCarrinho(jogoSelecionado);

            return RedirectToAction(nameof(Index));
        }

        // Helper que monta o ViewModel básico
        private CarrinhoCompraViewModel CriarViewModel()
        {
            var itens = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItems = itens;

            var total = _carrinhoCompra.GetCarrinhoCompraTotal();

            return new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                CarrinhoCompraTotal = total,
                CodigoCupom = null,
                Desconto = 0m,
                TotalComDesconto = total
            };
        }
    }
}
