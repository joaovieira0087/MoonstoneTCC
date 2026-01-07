using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.AspNetCore.Authorization;

namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminVagasController : Controller
    {
        private readonly AppDbContext _context;

        public AdminVagasController(AppDbContext context)
        {
            _context = context;
        }

        // LISTA DE VAGAS
        public async Task<IActionResult> Index()
        {
            var vagas = await _context.VagasEmprego
                .Include(v => v.Perguntas)
                .Include(v => v.Candidaturas)
                .OrderByDescending(v => v.DataCriacao)
                .ToListAsync();

            var entrevistas = await _context.EntrevistasAgendadas
                .Include(e => e.Candidatura).ThenInclude(c => c.Usuario)
                .Include(e => e.Candidatura).ThenInclude(c => c.Vaga)
                .Where(e => e.DataHoraEntrevista >= DateTime.Now)
                .OrderBy(e => e.DataHoraEntrevista)
                .ToListAsync();

            ViewBag.Entrevistas = entrevistas;

            return View(vagas);
        }


        // CRIAR NOVA VAGA
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VagaEmprego vaga, List<string> perguntas)
        {
            vaga.DataCriacao = DateTime.Now;
            vaga.Perguntas = perguntas.Select(p => new PerguntaVaga { Texto = p }).ToList();

            _context.VagasEmprego.Add(vaga);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // VER CANDIDATOS DE UMA VAGA
        public async Task<IActionResult> Candidatos(int id, string status = "Todos", string nome = "")
        {
            var vaga = await _context.VagasEmprego
                .Include(v => v.Candidaturas)
                    .ThenInclude(c => c.Usuario)
                .Include(v => v.Candidaturas)
                    .ThenInclude(c => c.Respostas)
                        .ThenInclude(r => r.Pergunta)
                .Include(v => v.Candidaturas)
                    .ThenInclude(c => c.Etapas)
                .Include(v => v.Candidaturas)
                    .ThenInclude(c => c.HistoricoStatus) 
                .Include(v => v.Candidaturas)
                    .ThenInclude(c => c.EntrevistaAgendada) 
                .Include(v => v.Candidaturas)
                    .ThenInclude(c => c.Mensagens)
                .FirstOrDefaultAsync(v => v.Id == id);



            if (vaga == null) return NotFound();

            var candidaturas = vaga.Candidaturas.AsQueryable();

            if (status != "Todos")
                candidaturas = candidaturas.Where(c => c.Status == status);

            if (!string.IsNullOrWhiteSpace(nome))
                candidaturas = candidaturas.Where(c => c.NomeCompleto.ToLower().Contains(nome.ToLower()));

            vaga.Candidaturas = candidaturas
                .OrderByDescending(c => c.DataEnvio)
                .ToList();

            ViewBag.StatusSelecionado = status;
            ViewBag.NomeBusca = nome;

            return View(vaga);
        }




        // ATUALIZAR STATUS DA CANDIDATURA
        [HttpPost]
        public async Task<IActionResult> AtualizarStatus(int candidaturaId, string novoStatus)
        {
            var candidatura = await _context.Candidaturas.FindAsync(candidaturaId);
            if (candidatura == null) return NotFound();

            if (candidatura.Status != novoStatus)
            {
                // Salvar histórico
                var historico = new HistoricoStatusCandidatura
                {
                    CandidaturaId = candidatura.Id,
                    DeStatus = candidatura.Status,
                    ParaStatus = novoStatus,
                    DataHora = DateTime.Now
                };
                _context.HistoricoStatusCandidaturas.Add(historico);
            }

            candidatura.Status = novoStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction("Candidatos", new { id = candidatura.VagaEmpregoId });
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarEtapa(int candidaturaId, string nomeEtapa, string observacoes)
        {
            var candidatura = await _context.Candidaturas
                .Include(c => c.Vaga)
                .FirstOrDefaultAsync(c => c.Id == candidaturaId);

            if (candidatura == null) return NotFound();

            var etapa = new EtapaProcessoSeletivo
            {
                CandidaturaId = candidaturaId,
                NomeEtapa = nomeEtapa,
                Observacoes = observacoes,
                Data = DateTime.Now
            };

            _context.EtapasProcessoSeletivo.Add(etapa);
            await _context.SaveChangesAsync();

            return RedirectToAction("Candidatos", new { id = candidatura.VagaEmpregoId });
        }

        [HttpPost]
        public async Task<IActionResult> SalvarComentarioInterno(int candidaturaId, string comentario)
        {
            var candidatura = await _context.Candidaturas.FindAsync(candidaturaId);
            if (candidatura == null) return NotFound();

            candidatura.ComentarioInterno = comentario;
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Comentário salvo com sucesso!";
            return RedirectToAction("Candidatos", new { id = candidatura.VagaEmpregoId });
        }

        [HttpPost]
        public async Task<IActionResult> SalvarFeedback(int candidaturaId, string feedback)
        {
            var candidatura = await _context.Candidaturas.FindAsync(candidaturaId);
            if (candidatura == null) return NotFound();

            candidatura.Feedback = feedback;
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Feedback salvo com sucesso!";
            return RedirectToAction("Candidatos", new { id = candidatura.VagaEmpregoId });
        }

        [HttpPost]
        public async Task<IActionResult> AgendarEntrevista(int candidaturaId, DateTime dataHora, string observacoes)
        {
            var candidatura = await _context.Candidaturas
                .Include(c => c.EntrevistaAgendada)
                .FirstOrDefaultAsync(c => c.Id == candidaturaId);

            if (candidatura == null) return NotFound();

            if (candidatura.EntrevistaAgendada == null)
            {
                candidatura.EntrevistaAgendada = new EntrevistaAgendada
                {
                    DataHoraEntrevista = dataHora,
                    Observacoes = observacoes
                };
            }
            else
            {
                candidatura.EntrevistaAgendada.DataHoraEntrevista = dataHora;
                candidatura.EntrevistaAgendada.Observacoes = observacoes;
            }

            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Entrevista agendada com sucesso!";
            return RedirectToAction("Candidatos", new { id = candidatura.VagaEmpregoId });
        }

        public async Task<PartialViewResult> EntrevistasAgendadas()
        {
            var entrevistas = await _context.EntrevistasAgendadas
                .Include(e => e.Candidatura)
                    .ThenInclude(c => c.Usuario)
                .Include(e => e.Candidatura)
                    .ThenInclude(c => c.Vaga)
                .Where(e => e.DataHoraEntrevista >= DateTime.Now)
                .OrderBy(e => e.DataHoraEntrevista)
                .ToListAsync();

            return PartialView("Partials/_EntrevistasAgendadas", entrevistas);
        }

        [HttpPost]
        public async Task<IActionResult> ResponderMensagem(int candidaturaId, string mensagem)
        {
            var candidatura = await _context.Candidaturas
                .Include(c => c.Mensagens)
                .FirstOrDefaultAsync(c => c.Id == candidaturaId);

            if (candidatura == null)
                return NotFound();

            candidatura.Mensagens ??= new List<MensagemCandidatura>();
            candidatura.Mensagens.Add(new MensagemCandidatura
            {
                CandidaturaId = candidaturaId,
                Texto = mensagem,
                Remetente = "Admin",
                DataHora = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Candidatos", new { id = candidatura.VagaEmpregoId }); // corrigido aqui
        }





        // GET: Editar Vaga
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vaga = await _context.VagasEmprego
                .Include(v => v.Perguntas)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vaga == null) return NotFound();

            return View(vaga);
        }

        // POST: Editar Vaga
        [HttpPost]
        public async Task<IActionResult> Edit(int id, VagaEmprego vagaEditada)
        {
            if (id != vagaEditada.Id) return NotFound();

            if (!ModelState.IsValid) return View(vagaEditada);

            var vaga = await _context.VagasEmprego
                .Include(v => v.Perguntas)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vaga == null) return NotFound();

            // Atualiza campos principais
            vaga.Titulo = vagaEditada.Titulo;
            vaga.Cargo = vagaEditada.Cargo;
            vaga.Descricao = vagaEditada.Descricao;
            vaga.Requisitos = vagaEditada.Requisitos;
            vaga.Salario = vagaEditada.Salario;
            vaga.DataEncerramento = vagaEditada.DataEncerramento;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Confirmar exclusão
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vaga = await _context.VagasEmprego.FirstOrDefaultAsync(v => v.Id == id);
            if (vaga == null) return NotFound();

            return View(vaga);
        }

        // POST: Excluir vaga
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vaga = await _context.VagasEmprego.FindAsync(id);
            if (vaga == null) return NotFound();

            _context.VagasEmprego.Remove(vaga);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }
}
