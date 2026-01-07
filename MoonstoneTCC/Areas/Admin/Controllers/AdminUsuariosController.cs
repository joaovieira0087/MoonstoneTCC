using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using MoonstoneTCC.Models;
using MoonstoneTCC.Services;


namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminUsuariosController : Controller
    {
        private readonly LoggerAdminService _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AdminUsuariosController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 1. Listar usuários
        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.ToList();
            Console.WriteLine($"Usuários carregados: {usuarios.Count}");
            foreach (var user in usuarios)
            {
                Console.WriteLine($"Usuário: {user.Email}");
            }
            return View(usuarios);
        }


        // 2. Criar usuário
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ModelState.AddModelError("", "Email e senha são obrigatórios.");
                return View();
            }

            var usuario = new IdentityUser { UserName = email, Email = email };
            var resultado = await _userManager.CreateAsync(usuario, senha);

            if (resultado.Succeeded)
            {
                await _logger.RegistrarAcaoAsync($"Criou o usuário: {email}");
                return RedirectToAction(nameof(Index));
            }

            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError("", erro.Description);
            }
            return View();
        }

        // 3. Editar usuário
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Email = email;
            usuario.UserName = email;
            var resultado = await _userManager.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                await _logger.RegistrarAcaoAsync($"Editou o e-mail do usuário: {usuario.Email}");
                return RedirectToAction(nameof(Index));
            }

            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError("", erro.Description);
            }
            return View(usuario);
        }

        // 4. Deletar usuário
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var resultado = await _userManager.DeleteAsync(usuario);

            if (resultado.Succeeded)
            {
                await _logger.RegistrarAcaoAsync($"Excluiu o usuário: {usuario.Email}");
                return RedirectToAction(nameof(Index));
            }

            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError("", erro.Description);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
