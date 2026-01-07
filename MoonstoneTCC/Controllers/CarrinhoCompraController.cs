using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.Services;
using MoonstoneTCC.ViewModels;
using System.Collections.Generic;    // Para Dictionary<string,int>

namespace MoonstoneTCC.Controllers
{
    public class CarrinhoCompraController : Controller
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;
        private readonly AppDbContext _context;
        private readonly ICepFreteService _frete;


        public CarrinhoCompraController(IJogoRepository jogoRepository,
                                        CarrinhoCompra carrinhoCompra,
                                        AppDbContext context, ICepFreteService frete)
        {
            _jogoRepository = jogoRepository;
            _carrinhoCompra = carrinhoCompra;
            _context = context;
            _frete = frete;
        }

        // GET: /CarrinhoCompra
        [HttpGet]
        public IActionResult Index(string cep = null)
        {
            var vm = CriarViewModel(cep, null);
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(string codigoCupom, string cep) // <- RECEBE o CEP
        {
            var vm = CriarViewModel(cep, codigoCupom); // <- recalcula desconto + frete juntos
            return View("Index", vm);
        }

        [Authorize]
        public IActionResult AdicionarItemNoCarrinhoCompra(int jogoId)
        {
            var jogoSelecionado = _jogoRepository.Jogos
                .FirstOrDefault(p => p.JogoId == jogoId);

            if (jogoSelecionado != null)
                _carrinhoCompra.AdicionarAoCarrinho(jogoSelecionado);
            TempData["MensagemSucesso"] = "Missão concluída! Jogo no carrinho!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult RemoverItemDoCarrinhoCompra(int jogoId)
        {
            var jogoSelecionado = _jogoRepository.Jogos
                .FirstOrDefault(p => p.JogoId == jogoId);

            if (jogoSelecionado != null)
                _carrinhoCompra.RemoverDoCarrinho(jogoSelecionado);
            TempData["MensagemSucesso"] = "Remoção confirmada. O carrinho ficou mais leve.";
            return RedirectToAction(nameof(Index));
        }

        // Helper que monta o ViewModel básico
        private CarrinhoCompraViewModel CriarViewModel()
        {
            var itens = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItems = itens;

            var total = _carrinhoCompra.GetCarrinhoCompraTotal();

            var economia = itens.Sum(item =>
            {
                decimal precoOriginal = 0;

                if (item.Jogo != null)
                    precoOriginal = item.Jogo.Preco;
                else if (item.Acessorio != null)
                    precoOriginal = item.Acessorio.Preco;

                var precoPago = item.PrecoUnitario;
                return (precoOriginal - precoPago) * item.Quantidade;
            });

            return new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                CarrinhoCompraTotal = total,
                CodigoCupom = null,
                Desconto = 0m,
                TotalComDesconto = total,
                 EconomiaTotal = economia
            };
        }

        [Authorize]
        [HttpPost]
        public IActionResult AdicionarMultiplosAoCarrinho(List<int> jogoIds)
        {
            foreach (var jogoId in jogoIds)
            {
                var jogo = _jogoRepository.Jogos.FirstOrDefault(j => j.JogoId == jogoId);
                if (jogo != null)
                {
                    _carrinhoCompra.AdicionarAoCarrinho(jogo);
                }
            }
            TempData["MensagemSucesso"] = "Todos os jogos foram teleportados para o carrinho!";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public IActionResult ComprarMultiplosAgora(List<int> jogoIds)
        {
            // Limpa o carrinho antes, se você quiser garantir que será só esses jogos
            _carrinhoCompra.LimparCarrinho();

            foreach (var jogoId in jogoIds)
            {
                var jogo = _jogoRepository.Jogos.FirstOrDefault(j => j.JogoId == jogoId);
                if (jogo != null)
                {
                    _carrinhoCompra.AdicionarAoCarrinho(jogo);
                }
            }
            TempData["MensagemSucesso"] = "Jogada iniciada! Agora é só finalizar o pagamento.";
            // Redireciona direto para o checkout
            return RedirectToAction("Checkout", "Pedido");
        }


        [Authorize]
        public IActionResult ComprarAgora(int jogoId)
        {
            var jogo = _jogoRepository.Jogos.FirstOrDefault(j => j.JogoId == jogoId);
            if (jogo != null)
            {
                _carrinhoCompra.LimparCarrinho(); // limpa carrinho anterior se quiser garantir compra única
                _carrinhoCompra.AdicionarAoCarrinho(jogo);
            }
            TempData["MensagemSucesso"] = "Jogada iniciada! Agora é só finalizar o pagamento.";
            return RedirectToAction("Checkout", "Pedido");
        }

        [Authorize]
        [HttpPost]
        public IActionResult ComprarOuAdicionar(int jogoId, int quantidade, string acao)
        {
            var jogo = _jogoRepository.Jogos.FirstOrDefault(j => j.JogoId == jogoId);
            if (jogo == null || quantidade <= 0) return RedirectToAction("Index");

            if (acao == "comprar")
            {
                _carrinhoCompra.LimparCarrinho();
            }

            for (int i = 0; i < quantidade; i++)
            {
                _carrinhoCompra.AdicionarAoCarrinho(jogo);
            }

            if (acao == "comprar")
                return RedirectToAction("Checkout", "Pedido");
            else
                return RedirectToAction("Index");
        }

        // acessorios carrinho 

        [Authorize]
        [HttpPost]
        public IActionResult ComprarOuAdicionarAcessorio(int acessorioId, int quantidade, string acao)
        {
            var acessorio = _context.Acessorios.FirstOrDefault(a => a.AcessorioId == acessorioId);
            if (acessorio == null || quantidade <= 0)
                return RedirectToAction("Index");

            if (acao == "comprar")
            {
                _carrinhoCompra.LimparCarrinho();
            }

            for (int i = 0; i < quantidade; i++)
            {
                _carrinhoCompra.AdicionarAoCarrinho(acessorio);
            }

            if (acao == "comprar")
                return RedirectToAction("Checkout", "Pedido");
            else
                return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult AdicionarItemNoCarrinhoCompraAcessorio(int acessorioId)
        {
            var acessorioSelecionado = _context.Acessorios
                .FirstOrDefault(a => a.AcessorioId == acessorioId);

            if (acessorioSelecionado != null)
                _carrinhoCompra.AdicionarAoCarrinho(acessorioSelecionado);

            TempData["MensagemSucesso"] = "Acessório adicionado ao carrinho!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult RemoverItemDoCarrinhoCompraAcessorio(int acessorioId)
        {
            var acessorioSelecionado = _context.Acessorios
                .FirstOrDefault(a => a.AcessorioId == acessorioId);

            if (acessorioSelecionado != null)
                _carrinhoCompra.RemoverDoCarrinho(acessorioSelecionado);

            TempData["MensagemSucesso"] = "Acessório removido do carrinho.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CotarFrete(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep)) return BadRequest("CEP obrigatório.");

            var vm = CriarViewModel();
            var qtd = vm.CarrinhoCompra.CarrinhoCompraItems.Sum(i => i.Quantidade);
            var cot = _frete.Calcular(cep, vm.TotalComDesconto, qtd);

            return Json(new { valor = cot.Valor.ToString("F2"), prazoDias = cot.PrazoDias, regiao = cot.Regiao });
        }


        private CarrinhoCompraViewModel CriarViewModel(string cep = null, string codigoCupom = null)
        {
            var itens = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItems = itens;

            var total = _carrinhoCompra.GetCarrinhoCompraTotal();

            var economia = itens.Sum(item =>
            {
                decimal precoOriginal = item.Jogo?.Preco ?? item.Acessorio?.Preco ?? 0m;
                return (precoOriginal - item.PrecoUnitario) * item.Quantidade;
            });

            var vm = new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                CarrinhoCompraTotal = total,
                CodigoCupom = null,
                Desconto = 0m,
                TotalComDesconto = total,
                EconomiaTotal = economia
            };

            // Cupom (mesma regra que você usa no POST)
            var cupons = new Dictionary<string, int>
                {
                    { "PROMO10", 10 },
                    { "PROMO20", 20 }
                };
            if (!string.IsNullOrWhiteSpace(codigoCupom) &&
                cupons.TryGetValue(codigoCupom.ToUpper(), out int pct))
            {
                vm.CodigoCupom = codigoCupom.ToUpper();
                vm.Desconto = vm.CarrinhoCompraTotal * pct / 100m;
                vm.TotalComDesconto = vm.CarrinhoCompraTotal - vm.Desconto;
            }

            // Frete baseado no TOTAL com desconto
            if (!string.IsNullOrWhiteSpace(cep))
            {
                var qtd = itens.Sum(i => i.Quantidade);
                var cot = _frete.Calcular(cep, vm.TotalComDesconto, qtd);

                vm.CepDestino = cep;
                vm.ValorFrete = cot.Valor;
                vm.PrazoFreteDias = cot.PrazoDias;
                vm.RegiaoDestino = cot.Regiao;
                // vm.TotalFinal agora já reflete desconto + frete
            }

            return vm;
        }
    }
}
