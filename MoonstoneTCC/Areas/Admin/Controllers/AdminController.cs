using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var ultimasAcoes = await _context.AcoesAdmin
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.DataHora)
                .Take(10)
                .ToListAsync();

            ViewBag.TotalUsuarios = _userManager.Users.Count();
            ViewBag.TotalJogos = await _context.Jogos.CountAsync();
            ViewBag.TotalCategorias = await _context.Categorias.CountAsync();
            ViewBag.TotalPerguntasPendentes = await _context.PerguntasUsuarios.CountAsync(p => !p.Respondido);
            ViewBag.TotalComunicados = await _context.Comunicados.CountAsync();

            return View(ultimasAcoes);
        }

        [HttpPost]
        public async Task<IActionResult> ExcluirAcao(int id)
        {
            var acao = await _context.AcoesAdmin.FindAsync(id);
            if (acao == null) return NotFound();

            _context.AcoesAdmin.Remove(acao);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Resumo do Dia
        [HttpGet]
        public async Task<IActionResult> ResumoDoDia(DateTime? data)
        {
            var diaSelecionado = data ?? DateTime.Today;

            // Se você estiver usando uma classe customizada como Usuario : IdentityUser
            // Troque IdentityUser por Usuario

            var cadastros = await _context.Users
                .Where(u => EF.Property<DateTime>(u, "DataCadastro").Date == diaSelecionado.Date)
                .ToListAsync();

            var pedidos = await _context.Pedidos
                .Where(p => EF.Property<DateTime>(p, "PedidoEnviado").Date == diaSelecionado.Date)
                .ToListAsync();

            var perguntas = await _context.PerguntasUsuarios
                .Where(p => p.DataEnvio.Date == diaSelecionado.Date)
                .ToListAsync();

            var edicoes = await _context.AcoesAdmin
                .Where(a => a.DataHora.Date == diaSelecionado.Date)
                .Include(a => a.Usuario)
                .ToListAsync();

            var resumo = new ResumoDiaViewModel
            {
                DataSelecionada = diaSelecionado,
                Cadastros = cadastros,
                Pedidos = pedidos,
                Perguntas = perguntas,
                Edicoes = edicoes
            };

            return View(resumo);
        }

        [HttpGet]
        public async Task<IActionResult> DashboardData(int days = 14)
        {
            if (days < 1) days = 14;
            var start = DateTime.Today.AddDays(-(days - 1));

            // Pedidos por dia
            var pedidosPorDia = await _context.Pedidos
                .Where(p => EF.Property<DateTime>(p, "PedidoEnviado") >= start)
                .GroupBy(p => EF.Property<DateTime>(p, "PedidoEnviado").Date)
                .Select(g => new { Data = g.Key, Qtde = g.Count() })
                .ToListAsync();

            // Cadastros por dia (Users precisa ter shadow prop DataCadastro – como você já usa no Resumo)
            var cadastrosPorDia = await _context.Users
                .Where(u => EF.Property<DateTime>(u, "DataCadastro") >= start)
                .GroupBy(u => EF.Property<DateTime>(u, "DataCadastro").Date)
                .Select(g => new { Data = g.Key, Qtde = g.Count() })
                .ToListAsync();

            // Monta labels contínuos (mesmo se dia não tiver dado)
            var labels = Enumerable.Range(0, days)
                .Select(i => start.AddDays(i))
                .Select(d => d.ToString("dd/MM", new CultureInfo("pt-BR")))
                .ToList();

            int getCount(List<dynamic> list, DateTime date) =>
                list.FirstOrDefault(x => (DateTime)x.Data == date)?.Qtde ?? 0;

            var orders = new List<int>();
            var users = new List<int>();
            for (int i = 0; i < days; i++)
            {
                var d = start.AddDays(i).Date;
                orders.Add(getCount(pedidosPorDia.Cast<dynamic>().ToList(), d));
                users.Add(getCount(cadastrosPorDia.Cast<dynamic>().ToList(), d));
            }

            // Top categorias por quantidade de jogos (top 8)
            var topCategorias = await _context.Jogos
                .Include(j => j.Categoria)
                .GroupBy(j => j.Categoria.CategoriaNome)
                .Select(g => new { Categoria = g.Key, Qtde = g.Count() })
                .OrderByDescending(x => x.Qtde)
                .Take(8)
                .ToListAsync();

            return Json(new
            {
                labels,
                orders,
                users,
                topCategorias = new
                {
                    labels = topCategorias.Select(t => t.Categoria).ToList(),
                    values = topCategorias.Select(t => t.Qtde).ToList()
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> ExportarAcoesCsv()
        {
            var acoes = await _context.AcoesAdmin
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.DataHora)
                .Take(1000)
                .ToListAsync();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Usuario,Acao,DataHora");
            foreach (var a in acoes)
            {
                var usuario = (a.Usuario?.UserName ?? "").Replace(",", " ");
                var acao = (a.Acao ?? "").Replace(",", " ");
                sb.AppendLine($"{usuario},{acao},{a.DataHora:yyyy-MM-dd HH:mm}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"acoes_admin_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }


    }

}

