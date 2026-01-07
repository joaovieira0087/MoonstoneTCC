using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Services;
using ReflectionIT.Mvc.Paging;

namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminJogosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LoggerAdminService _logger;


        public AdminJogosController(AppDbContext context, LoggerAdminService logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: Admin/AdminJogos
        //public async Task<IActionResult> Index()
        //{
        //   var appDbContext = _context.Jogos.Include(j => j.Categoria);
        //  return View(await appDbContext.ToListAsync());
        // }

        // GET: Admin/AdminJogos/Details/5

        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "Nome")
        {
            var resultado = _context.Jogos.Include(l => l.Categoria).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                resultado = resultado.Where(p => p.Nome.Contains(filter));
            }

            var model = await PagingList.CreateAsync(resultado, 5, pageindex, sort, "Nome");
            model.RouteValue = new RouteValueDictionary { { "filter", filter } };
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogo = await _context.Jogos
                .Include(j => j.Categoria)
                .FirstOrDefaultAsync(m => m.JogoId == id);
            if (jogo == null)
            {
                return NotFound();
            }

            return View(jogo);
        }

        // GET: Admin/AdminJogos/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Desenvolvedoras = new SelectList(_context.Desenvolvedoras, "DesenvolvedoraId", "Nome");
            ViewBag.CategoriaId = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome");

            return View();
        }


        // POST: Admin/AdminJogos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JogoId,Nome,DescricaoCurta,DescricaoDetalhada,Preco,ImagemUrl,ImagemThumbnailUrl,IsJogoPreferido,EmEstoque,Plataformas,Genero,ClassificacaoEtaria,ClassificacaoEtariaImagemUrl,InformacoesExtras,ImagensAdicionais,ImagensAdicionais2,ImagensAdicionais3,CategoriaId,DesenvolvedoraId,TrailerYoutubeUrl,QuantidadeEstoque,EstoqueMinimoAlerta")] Jogo jogo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jogo);
                await _context.SaveChangesAsync();
                await _logger.RegistrarAcaoAsync($"Criou um novo jogo: {jogo.Nome}");
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", jogo.CategoriaId);
            ViewBag.Desenvolvedoras = new SelectList(_context.Desenvolvedoras, "DesenvolvedoraId", "Nome", jogo.DesenvolvedoraId);

            return View(jogo);
        }


        // GET: Admin/AdminJogos/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null) return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", jogo.CategoriaId);
            ViewData["DesenvolvedoraId"] = new SelectList(_context.Desenvolvedoras, "DesenvolvedoraId", "Nome", jogo.DesenvolvedoraId);

            return View(jogo);
        }




        // POST: Admin/AdminJogos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var jogoDb = await _context.Jogos.FirstOrDefaultAsync(j => j.JogoId == id);
            if (jogoDb == null) return NotFound();

            var estavaZerado = jogoDb.QuantidadeEstoque == 0;

            if (await TryUpdateModelAsync(jogoDb, "",
                j => j.Nome,
                j => j.DescricaoCurta,
                j => j.DescricaoDetalhada,
                j => j.Preco,
                j => j.ImagemUrl,
                j => j.ImagemThumbnailUrl,
                j => j.IsJogoPreferido,
                j => j.Plataformas,
                j => j.Genero,
                j => j.ClassificacaoEtaria,
                j => j.ClassificacaoEtariaImagemUrl,
                j => j.InformacoesExtras,
                j => j.ImagensAdicionais,
                j => j.ImagensAdicionais2,
                j => j.ImagensAdicionais3,
                j => j.CategoriaId,
                j => j.DesenvolvedoraId,
                j => j.TrailerYoutubeUrl,
                j => j.QuantidadeEstoque,     // <- agora vem do form
                j => j.EstoqueMinimoAlerta,   // <- idem
                j => j.PrecoPromocional,
                j => j.PorcentagemDesconto,
                j => j.EmEstoque))
            {
                // Garante consistência: EmEstoque segue a quantidade
                jogoDb.EmEstoque = jogoDb.QuantidadeEstoque > 0;

                await _context.SaveChangesAsync();
                await _logger.RegistrarAcaoAsync($"Editou o jogo: {jogoDb.Nome}");

                // Se voltou ao estoque, notifica interessados (mesma lógica do AdminEstoqueController)
                if (estavaZerado && jogoDb.QuantidadeEstoque > 0)
                {
                    await NotificarInteressadosSeVoltouEstoque(jogoDb); // ver método abaixo
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", jogoDb.CategoriaId);
            ViewData["DesenvolvedoraId"] = new SelectList(_context.Desenvolvedoras, "DesenvolvedoraId", "Nome", jogoDb.DesenvolvedoraId);
            return View(jogoDb);
        }

        // EXTRA: extraia a lógica de notificação para um serviço comum
        private async Task NotificarInteressadosSeVoltouEstoque(Jogo jogo)
        {
            var interessados = await _context.AvisosEstoque
                .Where(a => a.JogoId == jogo.JogoId && !a.Avisado)
                .ToListAsync();

            // ... envie pelo SignalR e registre em NotificacoesEstoque exatamente como no AdminEstoqueController ...
            // marque a.Avisado = true e salve
        }



        // GET: Admin/AdminJogos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogo = await _context.Jogos
                .Include(j => j.Categoria)
                .FirstOrDefaultAsync(m => m.JogoId == id);
            if (jogo == null)
            {
                return NotFound();
            }

            return View(jogo);
        }

        // POST: Admin/AdminJogos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo != null)
            {
                _context.Jogos.Remove(jogo);
                await _context.SaveChangesAsync();

                await _logger.RegistrarAcaoAsync($"Excluiu o jogo: {jogo.Nome}");
            }

            return RedirectToAction(nameof(Index));
        }


        private bool JogoExists(int id)
        {
            return _context.Jogos.Any(e => e.JogoId == id);
        }
    }
}
