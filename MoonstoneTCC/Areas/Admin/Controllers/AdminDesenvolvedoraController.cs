using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DesenvolvedoraController : Controller
    {
        private readonly AppDbContext _context;

        public DesenvolvedoraController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var desenvolvedoras = _context.Desenvolvedoras.ToList();
            return View(desenvolvedoras);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Desenvolvedora desenvolvedora)
        {
            if (!ModelState.IsValid)
                return View(desenvolvedora);

            _context.Desenvolvedoras.Add(desenvolvedora);
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Desenvolvedora criada com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var desenvolvedora = _context.Desenvolvedoras.FirstOrDefault(d => d.DesenvolvedoraId == id);
            if (desenvolvedora == null) return NotFound();
            return View(desenvolvedora);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Desenvolvedora desenvolvedora)
        {
            if (!ModelState.IsValid)
            {
                // Recarrega a lista de desenvolvedoras se for necessário manter dropdowns na view
                ViewBag.Desenvolvedoras = _context.Desenvolvedoras
                    .Select(d => new SelectListItem
                    {
                        Value = d.DesenvolvedoraId.ToString(),
                        Text = d.Nome
                    }).ToList();

                return View(desenvolvedora);
            }

            var desenvolvedoraExistente = await _context.Desenvolvedoras.FindAsync(desenvolvedora.DesenvolvedoraId);
            if (desenvolvedoraExistente == null)
                return NotFound();

            desenvolvedoraExistente.Nome = desenvolvedora.Nome;
            desenvolvedoraExistente.Descricao = desenvolvedora.Descricao;
            desenvolvedoraExistente.FotoPerfilUrl = desenvolvedora.FotoPerfilUrl;

            // ...adicione mais campos se houver outros

            _context.Desenvolvedoras.Update(desenvolvedoraExistente);
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Desenvolvedora atualizada com sucesso!";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var desenvolvedora = _context.Desenvolvedoras.FirstOrDefault(d => d.DesenvolvedoraId == id);
            if (desenvolvedora == null) return NotFound();

            _context.Desenvolvedoras.Remove(desenvolvedora);
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Desenvolvedora excluída.";
            return RedirectToAction("Index");
        }
    }
}

