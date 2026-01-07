using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ListaJogoController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ListaJogoController(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Mostra todas as listas do usuário
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var listas = await _context.ListasJogos
            .Where(l => l.UsuarioId == user.Id)
            .Include(l => l.Jogos)
                .ThenInclude(j => j.Jogo)
            .ToListAsync();

        return View(listas);
    }

    // Cria uma nova lista e opcionalmente já adiciona um jogo nela
    [HttpPost]
    public async Task<IActionResult> CriarLista(string nomeLista, int? jogoId)
    {
        var user = await _userManager.GetUserAsync(User);

        var total = await _context.ListasJogos.CountAsync(l => l.UsuarioId == user.Id);
        if (total >= 20)
        {
            TempData["Mensagem"] = "Você atingiu o limite de 20 listas.";
            return RedirectToAction("Index");
        }

        var novaLista = new ListaJogo
        {
            Nome = nomeLista,
            UsuarioId = user.Id
        };

        _context.ListasJogos.Add(novaLista);
        await _context.SaveChangesAsync();

        if (jogoId.HasValue)
        {
            _context.ItensListaJogos.Add(new ItemListaJogo
            {
                ListaJogoId = novaLista.ListaJogoId,
                JogoId = jogoId.Value
            });

            await _context.SaveChangesAsync();

            // Buscar nome do jogo
            var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.JogoId == jogoId.Value);
            string nomeJogo = jogo?.Nome ?? "Jogo desconhecido";

            TempData["MensagemSucesso"] = $"Lista {nomeLista} criada e o jogo {nomeJogo} foi adicionado a ela!";
            return RedirectToAction("Details", "Jogo", new { jogoId = jogoId.Value });
        }

        TempData["MensagemSucesso"] = $"Lista {nomeLista} criada com sucesso!";
        return RedirectToAction("Index");
    }


    // Adiciona um jogo a uma lista específica
    [HttpPost]
    public async Task<IActionResult> AdicionarJogo(int listaId, int jogoId)
    {
        var existe = await _context.ItensListaJogos
            .AnyAsync(i => i.ListaJogoId == listaId && i.JogoId == jogoId);

        if (!existe)
        {
            _context.ItensListaJogos.Add(new ItemListaJogo
            {
                ListaJogoId = listaId,
                JogoId = jogoId
            });

            await _context.SaveChangesAsync();

            // Buscar nome do jogo e da lista para a mensagem
            var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.JogoId == jogoId);
            var lista = await _context.ListasJogos.FirstOrDefaultAsync(l => l.ListaJogoId == listaId);

            string nomeJogo = jogo?.Nome ?? "Jogo desconhecido";
            string nomeLista = lista?.Nome ?? "Lista desconhecida";

            TempData["MensagemSucesso"] = $"O jogo {nomeJogo} foi adicionado à lista {nomeLista} com sucesso!";
        }
        else
        {
            var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.JogoId == jogoId);
            var lista = await _context.ListasJogos.FirstOrDefaultAsync(l => l.ListaJogoId == listaId);

            string nomeJogo = jogo?.Nome ?? "Jogo";
            string nomeLista = lista?.Nome ?? "lista";

            TempData["Mensagem"] = $"O jogo <strong>{nomeJogo}</strong> já está na lista <strong>{nomeLista}</strong>.";
        }

        return RedirectToAction("Details", "Jogo", new { jogoId });
    }


    // Atualiza a lista marcando ou desmarcando o jogo (via checkbox)
    [HttpPost]
    public async Task<IActionResult> AtualizarLista(int listaId, int jogoId, bool adicionar)
    {
        var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.JogoId == jogoId);
        var lista = await _context.ListasJogos.FirstOrDefaultAsync(l => l.ListaJogoId == listaId);

        string nomeJogo = jogo?.Nome ?? "Jogo";
        string nomeLista = lista?.Nome ?? "Lista";

        if (adicionar)
        {
            var existe = await _context.ItensListaJogos
                .AnyAsync(x => x.ListaJogoId == listaId && x.JogoId == jogoId);

            if (!existe)
            {
                _context.ItensListaJogos.Add(new ItemListaJogo
                {
                    ListaJogoId = listaId,
                    JogoId = jogoId
                });

                await _context.SaveChangesAsync();

                TempData["MensagemSucesso"] = $"O jogo {nomeJogo} foi adicionado à lista {nomeLista}.";
            }
        }
        else
        {
            var item = await _context.ItensListaJogos
                .FirstOrDefaultAsync(x => x.ListaJogoId == listaId && x.JogoId == jogoId);

            if (item != null)
            {
                _context.ItensListaJogos.Remove(item);
                await _context.SaveChangesAsync();

                TempData["MensagemSucesso"] = $"O jogo {nomeJogo} foi removido da lista {nomeLista}.";
            }
        }

        return Ok();
    }


    // Remove uma lista inteira do usuário
    [HttpPost]
    public async Task<IActionResult> ExcluirLista(int listaId)
    {
        var lista = await _context.ListasJogos
            .Include(l => l.Jogos)
            .FirstOrDefaultAsync(l => l.ListaJogoId == listaId);

        if (lista == null)
            return NotFound();

        int qtdJogos = lista.Jogos.Count;
        string nomeLista = lista.Nome;

        _context.ItensListaJogos.RemoveRange(lista.Jogos);
        _context.ListasJogos.Remove(lista);
        await _context.SaveChangesAsync();

        TempData["MensagemSucesso"] = $"A lista {nomeLista} (com {qtdJogos} jogo{(qtdJogos == 1 ? "" : "s")}) foi excluída com sucesso!";

        return RedirectToAction("Index");
    }


    // Remove um jogo específico de uma lista
    [HttpPost]
    public async Task<IActionResult> RemoverJogoDaLista(int listaId, int jogoId)
    {
        var item = await _context.ItensListaJogos
            .FirstOrDefaultAsync(i => i.ListaJogoId == listaId && i.JogoId == jogoId);

        if (item != null)
        {
            var lista = await _context.ListasJogos
                .FirstOrDefaultAsync(l => l.ListaJogoId == listaId);

            var jogo = await _context.Jogos
                .FirstOrDefaultAsync(j => j.JogoId == jogoId);

            string nomeLista = lista?.Nome ?? "Lista desconhecida";
            string nomeJogo = jogo?.Nome ?? "Jogo desconhecido";

            _context.ItensListaJogos.Remove(item);
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = $"O jogo {nomeJogo} foi removido da lista {nomeLista}!";
        }

        return RedirectToAction("Index");
    }


    [AllowAnonymous]
    [HttpGet("/Lista/Ver/{id}")]
    public async Task<IActionResult> Ver(int id)
    {
        var lista = await _context.ListasJogos
            .Include(l => l.Jogos).ThenInclude(j => j.Jogo)
            .Include(l => l.Usuario)
            .FirstOrDefaultAsync(l => l.ListaJogoId == id);

        if (lista == null || !lista.EPublica)
            return NotFound();

        return View("VerListaPublica", lista);
    }

    // LISTA PUBLICA OU PRIVADA 

    [HttpPost]
    public async Task<IActionResult> AlternarPrivacidade(int listaId)
    {
        var user = await _userManager.GetUserAsync(User);

        var lista = await _context.ListasJogos
            .FirstOrDefaultAsync(l => l.ListaJogoId == listaId && l.UsuarioId == user.Id);

        if (lista == null)
            return NotFound();

        lista.EPublica = !lista.EPublica;
        await _context.SaveChangesAsync();

        if (lista.EPublica)
        {
            TempData["MensagemSucesso"] = "Sua lista agora é PÚBLICA Todos poderão visualizar.";
        }
        else
        {
            TempData["MensagemSucesso"] = "Sua lista agora é PRIVADA  Somente você poderá visualizar.";
        }

        return RedirectToAction("Index");
    }


    // POST: /Lista/Clonar
    [HttpPost]
    public async Task<IActionResult> CopiarLista(int listaId)
    {
        // Usuário logado
        var usuarioAtual = await _userManager.GetUserAsync(User);
        if (usuarioAtual == null) return Unauthorized();

        // Lista a clonar (pública, de outro usuário)
        var listaOriginal = await _context.ListasJogos
            .Include(l => l.Jogos)
            .FirstOrDefaultAsync(l => l.ListaJogoId == listaId && l.EPublica);

        if (listaOriginal == null)
            return NotFound();

        if (listaOriginal.UsuarioId == usuarioAtual.Id)
        {
            TempData["Mensagem"] = "Essa já é a sua própria lista 😉";
            return RedirectToAction("Ver", new { id = listaId });
        }

        // Limite de 20 listas
        var totalListas = await _context.ListasJogos.CountAsync(l => l.UsuarioId == usuarioAtual.Id);
        if (totalListas >= 20)
        {
            TempData["Mensagem"] = "Você já atingiu o limite de 20 listas.";
            return RedirectToAction("Ver", new { id = listaId });
        }

        // Garante nome único
        var nomeClonado = listaOriginal.Nome;
        var existeNome = await _context.ListasJogos.AnyAsync(l =>
            l.UsuarioId == usuarioAtual.Id && l.Nome == nomeClonado);

        if (existeNome)
            nomeClonado += " (cópia)";

        // Cria lista
        var novaLista = new ListaJogo
        {
            Nome = nomeClonado,
            UsuarioId = usuarioAtual.Id,
            EPublica = false // inicia privada, mas você decide
        };
        _context.ListasJogos.Add(novaLista);
        await _context.SaveChangesAsync();

        // Copia itens
        var itens = listaOriginal.Jogos.Select(j => new ItemListaJogo
        {
            ListaJogoId = novaLista.ListaJogoId,
            JogoId = j.JogoId
        });
        _context.ItensListaJogos.AddRange(itens);
        await _context.SaveChangesAsync();

        TempData["MensagemSucesso"] = $"Lista \"{listaOriginal.Nome}\" copiada!";
        return RedirectToAction("Index");
    }
}
