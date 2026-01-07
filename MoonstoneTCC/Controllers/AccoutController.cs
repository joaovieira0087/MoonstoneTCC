using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.ViewModels;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByNameAsync(loginVM.UserName);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginVM.ReturnUrl))
                        return RedirectToAction("Index", "Home");

                    return Redirect(loginVM.ReturnUrl);
                }
            }

            ModelState.AddModelError("", "Falha ao realizar o login!");
            return View(loginVM);
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Member");
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Perfil()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var ultimoPedido = await _context.Pedidos
                .Where(p => p.Email == user.Email)
                .OrderByDescending(p => p.PedidoEnviado)
                .FirstOrDefaultAsync();

            var totalPedidos = await _context.Pedidos.CountAsync(p => p.Email == user.Email);

            // Total de jogos comprados (soma das quantidades)
            var totalJogosComprados = await _context.PedidoDetalhes
                .Where(pd => pd.Pedido.Email == user.Email)
                .SumAsync(pd => pd.Quantidade);

            // Total de jogos avaliados (por Id do usuário logado)
            var totalJogosAvaliados = await _context.ComentariosJogo
                .CountAsync(c => c.UsuarioId == user.Id);

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
                UltimoPedido = ultimoPedido // <- aqui!
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Editar()
        {
            var user = await _userManager.GetUserAsync(User);
            var endereco = await _context.EnderecosEntrega
                .FirstOrDefaultAsync(e => e.UserId == user.Id && e.EnderecoPadrao);

            var model = new UserEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Nome = endereco?.Nome,
                Sobrenome = endereco?.Sobrenome,
                Telefone = endereco?.Telefone,
                CPF = "" // Se quiser salvar, você precisa incluir isso na model
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);

            // Verifica se vai alterar a senha
            if (!string.IsNullOrEmpty(model.NovaSenha))
            {
                if (string.IsNullOrEmpty(model.SenhaAtual))
                {
                    ModelState.AddModelError("SenhaAtual", "Informe a senha atual.");
                    return View(model);
                }

                var valid = await _userManager.CheckPasswordAsync(user, model.SenhaAtual);
                if (!valid)
                {
                    ModelState.AddModelError("SenhaAtual", "Senha atual incorreta.");
                    return View(model);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resultSenha = await _userManager.ResetPasswordAsync(user, token, model.NovaSenha);
                if (!resultSenha.Succeeded)
                {
                    foreach (var error in resultSenha.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }
            }

            // Atualiza dados do Identity
            user.UserName = model.UserName;
            user.Email = model.Email;
            await _userManager.UpdateAsync(user);

            // Atualiza dados do EnderecoEntrega
            var endereco = await _context.EnderecosEntrega
                .FirstOrDefaultAsync(e => e.UserId == user.Id && e.EnderecoPadrao);

            if (endereco != null)
            {
                endereco.Nome = model.Nome;
                endereco.Sobrenome = model.Sobrenome;
                endereco.Telefone = model.Telefone;
                // Se quiser CPF aqui, adicione na model
                await _context.SaveChangesAsync();
            }

            TempData["MensagemSucesso"] = "Alterações salvas com sucesso!";
            return RedirectToAction("Editar");

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ExcluirConta()
        {
            var user = await _userManager.GetUserAsync(User);

            var totalPedidos = await _context.Pedidos.CountAsync(p => p.UserId == user.Id);
            var totalGasto = await _context.Pedidos
                .Where(p => p.UserId == user.Id)
                .SumAsync(p => (decimal?)p.PedidoTotal) ?? 0;

            var dataCadastro = await _context.Pedidos
                .Where(p => p.UserId == user.Id)
                .OrderBy(p => p.PedidoEnviado)
                .Select(p => (DateTime?)p.PedidoEnviado)
                .FirstOrDefaultAsync() ?? DateTime.Now;

            var tempoNaPlataforma = DateTime.Now - dataCadastro;

            var model = new ExcluirContaViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                TotalPedidos = totalPedidos,
                TotalGasto = totalGasto,
                DataCadastro = dataCadastro,
                TempoNaPlataforma = tempoNaPlataforma
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusaoConta(string motivo)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Salva o motivo na tabela de Exclusão
            if (!string.IsNullOrEmpty(motivo))
            {
                var log = new ExclusaoConta
                {
                    UsuarioId = user.Id,
                    Email = user.Email,
                    Motivo = motivo,
                    DataHora = DateTime.Now
                };

                _context.Add(log);
                await _context.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(user);

            TempData["MensagemSucesso"] = "Conta excluída com sucesso.";
            return RedirectToAction("Index", "Home");
        }

    }

}

