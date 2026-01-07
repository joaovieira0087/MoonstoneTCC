using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Context;
using Microsoft.EntityFrameworkCore;


[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminComentariosController : Controller
{
    private readonly AppDbContext _context;

    public AdminComentariosController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string data, string usuario, string jogo, string comentario, int? avaliacao, string curtidas)

    {
        var comentarios = _context.ComentariosJogo
            .Include(c => c.Usuario)
            .Include(c => c.Jogo)
            .AsQueryable();

        if (!string.IsNullOrEmpty(data) && DateTime.TryParse(data, out var dataParsed))
        {
            comentarios = comentarios.Where(c => c.Data.Date == dataParsed.Date);
        }

        if (!string.IsNullOrEmpty(usuario))
        {
            usuario = usuario.ToLower().Trim();
            comentarios = comentarios.Where(c =>
                c.Usuario.UserName.ToLower().Contains(usuario) ||
                c.Usuario.Email.ToLower().Contains(usuario));
        }

        if (!string.IsNullOrEmpty(jogo))
        {
            jogo = jogo.ToLower().Trim();
            comentarios = comentarios.Where(c => c.Jogo.Nome.ToLower().Contains(jogo));
        }

        if (!string.IsNullOrEmpty(comentario))
        {
            comentario = comentario.ToLower().Trim();
            comentarios = comentarios.Where(c => c.Texto.ToLower().Contains(comentario));
        }

        if (avaliacao.HasValue)
        {
            comentarios = comentarios.Where(c => c.Avaliacao == avaliacao.Value);
        }

        var lista = await comentarios.OrderByDescending(c => c.Data).ToListAsync();

        // Pega total de curtidas por comentário
        var curtidasPorComentario = await _context.ComentarioCurtidas
            .GroupBy(cc => cc.ComentarioId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        ViewBag.CurtidasPorComentario = curtidasPorComentario;
        return View(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Excluir(int id)
    {
        var comentario = await _context.ComentariosJogo.FindAsync(id);
        if (comentario != null)
        {
            _context.ComentariosJogo.Remove(comentario);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}
