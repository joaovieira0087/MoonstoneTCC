using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MoonstoneTCC.Controllers
{
    public class VagasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public VagasController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // LISTAR VAGAS ABERTAS
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var vagas = await _context.VagasEmprego
                .OrderByDescending(v => v.DataCriacao)
                .ToListAsync();
            return View(vagas);
        }

        // DETALHES DA VAGA
        [AllowAnonymous]
        public async Task<IActionResult> Detalhes(int id)
        {
            var vaga = await _context.VagasEmprego
                .Include(v => v.Perguntas)
                .Include(v => v.Candidaturas) 
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vaga == null) return NotFound();

            return View(vaga);
        }


        // FORMULÁRIO DE CANDIDATURA
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Candidatar(int id)
        {
            var vaga = await _context.VagasEmprego
                .Include(v => v.Perguntas)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vaga == null) return NotFound();

            var model = new Candidatura
            {
                VagaEmpregoId = vaga.Id
            };

            ViewBag.TituloVaga = vaga.Titulo;
            ViewBag.VagaId = vaga.Id;
            ViewBag.Perguntas = vaga.Perguntas.ToList();

            return View(model);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Candidatar(int id, Candidatura model, List<string> respostas)
        {
            var user = await _userManager.GetUserAsync(User);
            var vaga = await _context.VagasEmprego
                .Include(v => v.Perguntas)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vaga == null) return NotFound();

            if (!ModelState.IsValid)
            {
                // Passa os dados necessários para reexibir a view corretamente
                ViewBag.TituloVaga = vaga.Titulo;
                ViewBag.VagaId = vaga.Id;
                ViewBag.Perguntas = vaga.Perguntas.ToList();

                return View(model); // Volta com os dados preenchidos
            }


            // Verifica se já se candidatou
            var jaCandidatou = await _context.Candidaturas
                .AnyAsync(c => c.UsuarioId == user.Id && c.VagaEmpregoId == id);
            if (jaCandidatou)
            {
                TempData["Erro"] = "Você já se candidatou para esta vaga.";
                return RedirectToAction("Status");
            }

            // Salvar dados
            model.Id = 0;
            model.UsuarioId = user.Id;
            model.DataEnvio = DateTime.Now;
            model.VagaEmpregoId = id;
            model.Status = "Pendente";

            _context.Candidaturas.Add(model);
            await _context.SaveChangesAsync();

            var perguntas = vaga.Perguntas.ToList();
            for (int i = 0; i < perguntas.Count; i++)
            {
                var resposta = new RespostaCandidatura
                {
                    CandidaturaId = model.Id,
                    PerguntaVagaId = perguntas[i].Id,
                    RespostaTexto = respostas[i]
                };
                _context.RespostasCandidatura.Add(resposta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Status");
        }





        // STATUS DAS MINHAS CANDIDATURAS
        [Authorize]
        public async Task<IActionResult> Status()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["Erro"] = "Usuário não autenticado.";
                return RedirectToAction("Login", "Account");
            }

            var candidaturas = await _context.Candidaturas
                .Include(c => c.Vaga)
                .Include(c => c.Respostas)
                .Include(c => c.HistoricoStatus)
                .Include(c => c.EntrevistaAgendada)
                .Include(c => c.Mensagens)
                .Where(c => c.UsuarioId == user.Id)
                .OrderByDescending(c => c.DataEnvio)
                .ToListAsync();

            if (!candidaturas.Any())
            {
                TempData["Info"] = "Você ainda não se candidatou a nenhuma vaga.";
            }

            return View(candidaturas);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EnviarMensagem(int candidaturaId, string mensagem)
        {
            var user = await _userManager.GetUserAsync(User);
            var candidatura = await _context.Candidaturas
                .FirstOrDefaultAsync(c => c.Id == candidaturaId && c.UsuarioId == user.Id);

            if (candidatura == null || string.IsNullOrWhiteSpace(mensagem))
            {
                TempData["Erro"] = "Mensagem inválida.";
                return RedirectToAction("Status");
            }

            var novaMensagem = new MensagemCandidatura
            {
                CandidaturaId = candidatura.Id,
                Remetente = "Candidato",
                Texto = mensagem,
                DataHora = DateTime.Now
            };

            _context.MensagensCandidatura.Add(novaMensagem);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Mensagem enviada com sucesso!";
            return RedirectToAction("Status");
        }

        [HttpPost]
        public async Task<IActionResult> CancelarCandidatura(int candidaturaId, string motivo)
        {
            var candidatura = await _context.Candidaturas.FindAsync(candidaturaId);
            if (candidatura == null) return NotFound();

            candidatura.Status = "Cancelada";
            candidatura.MotivoCancelamento = motivo;
            candidatura.DataCancelamento = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Info"] = "Candidatura cancelada com sucesso.";
            return RedirectToAction("Status");
        }



    }
}

