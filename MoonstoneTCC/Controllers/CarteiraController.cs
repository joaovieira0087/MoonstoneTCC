// Controllers/CarteiraController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Models.ViewModels;
using MoonstoneTCC.Services;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class CarteiraController : Controller
    {
        private readonly ICarteiraService _carteira;
        private readonly UserManager<IdentityUser> _userManager;

        public CarteiraController(ICarteiraService carteira, UserManager<IdentityUser> userManager)
        {
            _carteira = carteira;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var carteira = await _carteira.ObterAsync(user.Id);

            // Se não existir, cria e retorna (o service já trata isso, mas garantimos aqui)
            if (carteira == null)
                carteira = await _carteira.ObterOuCriarAsync(user.Id);

            return View(carteira);
        }

        [HttpGet]
        public IActionResult AdicionarSaldo()
        {
            return View(new DepositoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarSaldo(DepositoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                // Passa o valor validado (model.Valor.Value pois é nullable)
                await _carteira.DepositarAsync(user.Id, model.Valor!.Value, "Depósito via Portal Web");

                TempData["MensagemSucesso"] = "Depósito realizado com sucesso! Seu saldo foi atualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Em produção, logar o erro (ex)
                ModelState.AddModelError("", "Ocorreu um erro ao processar o depósito. Tente novamente.");
                return View(model);
            }
        }
    }
}