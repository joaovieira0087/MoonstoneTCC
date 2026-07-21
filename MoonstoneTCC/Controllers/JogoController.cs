using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;

namespace MoonstoneTCC.Controllers
{
    public class JogoController : Controller
    {
        private readonly IJogoRepository _jogoRepository;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public JogoController(IJogoRepository jogoRepository, UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _jogoRepository = jogoRepository;
            _userManager = userManager;
            _context = context;
        }


        //public IActionResult List(string categoria)
        //{
        //    IEnumerable<Jogo> jogos;
        //    string categoriaAtual;

        //    if (string.IsNullOrEmpty(categoria))
        //    {
        //        jogos = _jogoRepository.Jogos.OrderBy(j => j.JogoId);
        //        categoriaAtual = "Jogos"; // Nome padrão
        //    }
        //    else
        //    {
        //        jogos = _jogoRepository.Jogos
        //            .Where(j => j.Categoria.CategoriaNome.Equals(categoria))
        //            .OrderBy(j => j.Nome);
        //        categoriaAtual = categoria; // Nome da categoria atual
        //    }

        //    // Atualizar o título dinâmico no dropdown
        //    ViewData["CategoriaAtual"] = categoriaAtual;

        //    var jogosListViewModel = new JogoListViewModel
        //    {
        //        Jogos = jogos,
        //        CategoriaAtual = categoriaAtual
        //    };

        //    return View(jogosListViewModel);
        //}

        public async Task<IActionResult> Details(int jogoId)
        {
             var jogo = await _context.Jogos
            .Include(j => j.Desenvolvedora)
            .FirstOrDefaultAsync(j => j.JogoId == jogoId);

            if (jogo == null)
                return NotFound();

            // 👉 Aqui é o lugar certo!
            var ranking = _jogoRepository.GetClassificacaoRanking(jogoId);
            ViewBag.Ranking = ranking;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                var favorito = await _context.Favoritos
                     .FirstOrDefaultAsync(f => f.UsuarioId == user.Id && f.JogoId == jogoId);

                ViewBag.EhFavorito = favorito != null;
                ViewBag.TagFavorito = favorito?.TagFavorito;

                var jaExiste = await _context.HistoricoVisualizacoes
                    .AnyAsync(h => h.UsuarioId == user.Id && h.JogoId == jogoId);

                if (!jaExiste)
                {
                    var historico = new HistoricoVisualizacao
                    {
                        UsuarioId = user.Id,
                        JogoId = jogoId,
                        DataVisualizacao = DateTime.Now
                    };

                    _context.HistoricoVisualizacoes.Add(historico);
                    await _context.SaveChangesAsync();
                }


                // Listas do usuário
                var listas = await _context.ListasJogos
                    .Include(l => l.Jogos)
                    .Where(l => l.UsuarioId == user.Id)
                    .ToListAsync();

                ViewBag.ListasDoUsuario = listas;

                var listasComEsseJogo = listas
                    .Where(l => l.Jogos.Any(j => j.JogoId == jogoId))
                    .Select(l => l.ListaJogoId)
                    .ToList();

                ViewBag.ListasComJogo = listasComEsseJogo;
            }



            // Aqui adiciona os comentários ordenados
            var comentarios = await _context.ComentariosJogo
            .Include(c => c.Usuario)
            .Where(c => c.JogoId == jogoId)
            .OrderByDescending(c => _context.ComentarioCurtidas.Count(cc => cc.ComentarioId == c.Id))
            .ThenByDescending(c => c.Data)
            .ToListAsync();

            ViewBag.Comentarios = comentarios;
            ViewBag.TotalAvaliacoes = comentarios.Count;
            ViewBag.MediaAvaliacao = comentarios.Any() ? comentarios.Average(c => c.Avaliacao) : 0;


            ViewBag.JogosRecomendados = _jogoRepository.GetJogosRecomendados(jogoId, 4);
            ViewBag.OutrosJogos = _context.Jogos
                .Where(j => j.JogoId != jogoId)
                .OrderBy(_ => Guid.NewGuid())
                .Take(4)
                .ToList();

            return View(jogo);
        }

        public IActionResult Search(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return RedirectToAction("List");

            string termoBusca = searchString.Trim().ToLower();

            var jogos = _jogoRepository.Jogos
                .Where(p => p.Nome.ToLower().Contains(termoBusca) ||
                            (p.Categoria != null && p.Categoria.CategoriaNome.ToLower().Contains(termoBusca)) ||
                            (!string.IsNullOrEmpty(p.Genero) && p.Genero.ToLower().Contains(termoBusca)))
                .ToList();

            var acessorios = _context.Acessorios
                .Where(a => a.Nome.ToLower().Contains(termoBusca))
                .ToList();

            var sugestoes = new List<string>();

            if (jogos.Any())
            {
                var categoriasIds = jogos.Select(j => j.CategoriaId).Distinct().ToList();
                sugestoes.AddRange(_context.Categorias.Where(c => categoriasIds.Contains(c.CategoriaId)).Select(c => c.CategoriaNome));
                sugestoes.AddRange(_context.Jogos.Where(j => categoriasIds.Contains(j.CategoriaId) && !j.Nome.ToLower().Contains(termoBusca)).Take(3).Select(j => j.Nome));
            }
            else
            {
                     var categoriasPrincipais = _context.Categorias
                    .OrderBy(c => c.CategoriaNome)
                    .Take(3)
                    .Select(c => c.CategoriaNome)
                    .ToList();
                sugestoes.AddRange(categoriasPrincipais);

                
                var jogosAleatorios = _context.Jogos
                    .OrderBy(x => Guid.NewGuid()) 
                    .Take(3)
                    .Select(j => j.Nome)
                    .ToList();
                sugestoes.AddRange(jogosAleatorios);
            }

            var termosFinais = sugestoes.Distinct().Where(s => s.ToLower() != termoBusca).Take(6).ToList();

            var viewModel = new JogoListViewModel
            {
                Jogos = jogos,
                Acessorios = acessorios,
                CategoriaAtual = (jogos.Any() || acessorios.Any()) ? $"Resultados para '{searchString}'" : "Nenhum item encontrado",
                SugestoesTermos = termosFinais
            };

            return View("~/Views/Jogo/List.cshtml", viewModel);
        }

        public IActionResult List(string categoria)
        {
            IEnumerable<Jogo> jogos;
            string categoriaAtual;

            if (string.IsNullOrEmpty(categoria))
            {
                jogos = _jogoRepository.Jogos.OrderBy(j => j.JogoId);
                categoriaAtual = "Jogos";
            }
            else
            {
                jogos = _jogoRepository.Jogos
                    .Where(j => j.Categoria.CategoriaNome.Equals(categoria))
                    .OrderBy(j => j.Nome);
                categoriaAtual = categoria;
            }

            ViewData["CategoriaAtual"] = categoriaAtual;

            var jogosListViewModel = new JogoListViewModel
            {
                Jogos = jogos,
                CategoriaAtual = categoriaAtual,
                SugestoesTermos = new List<string>()
            };

            return View(jogosListViewModel);
        }



        // RECOMENDAÇÕES NA HORA DE BUSCAR
        [HttpGet]
        public async Task<JsonResult> BuscarSugestoes(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return Json(new List<object>());

            var termoLower = termo.ToLower();

            var sugestoesJogos = _context.Jogos
                .Where(j =>
                    j.Nome.ToLower().Contains(termoLower) ||
                    (!string.IsNullOrEmpty(j.DescricaoDetalhada) && j.DescricaoDetalhada.ToLower().Contains(termoLower)) ||
                    (!string.IsNullOrEmpty(j.Genero) && j.Genero.ToLower().Contains(termoLower)) ||
                    (!string.IsNullOrEmpty(j.Plataformas) && j.Plataformas.ToLower().Contains(termoLower))
                )
                .Select(j => new {
                    Tipo = "Jogo",
                    Id = j.JogoId.ToString(), // 🔁 convertendo para string
                    Nome = j.Nome,
                    Sub = j.Genero,
                    Imagem = j.ImagemThumbnailUrl
                });

            var sugestoesDesenvolvedoras = _context.Desenvolvedoras
                .Where(d => d.Nome.ToLower().Contains(termoLower))
                .Select(d => new {
                    Tipo = "Desenvolvedora",
                    Id = d.DesenvolvedoraId.ToString(), // 🔁 convertendo para string
                    Nome = d.Nome,
                    Sub = "Desenvolvedora",
                    Imagem = d.FotoPerfilUrl
                });

            var sugestoesUsuarios = await _userManager.Users
                .Where(u => u.UserName.ToLower().Contains(termoLower))
                .Select(u => new {
                    Tipo = "Usuario",
                    Id = u.Id, // já é string
                    Nome = u.UserName,
                    Sub = "Usuário",
                    Imagem = "/img/user-default.png"
                }).ToListAsync();

            var resultado = await sugestoesJogos
                .Concat(sugestoesDesenvolvedoras)
                .Take(5)
                .ToListAsync();

            resultado.AddRange(sugestoesUsuarios.Take(3));

            return Json(resultado);
        }

        public List<Jogo> GetJogosMaisBuscados(int quantidade)
        {
            return _context.HistoricoVisualizacoes
                .GroupBy(h => h.JogoId)
                .OrderByDescending(g => g.Count())
                .Take(quantidade)
                .Select(g => g.Key)
                .Join(_context.Jogos, id => id, jogo => jogo.JogoId, (id, jogo) => jogo)
                .ToList();
        }

        public IActionResult Promocoes()
        {
            var jogosEmPromocao = _context.Jogos
                .Where(j => j.PrecoPromocional != null)
                .ToList();

            return View(jogosEmPromocao);
        }





        public IActionResult Recomendados(int jogoId)
        {
            var jogosRecomendados = _jogoRepository.GetJogosRecomendados(jogoId, 4);
            return PartialView("_JogosRecomendados", jogosRecomendados);
        }

        public IActionResult OutrosJogos(int jogoId)
        {
            // Retorna jogos aleatórios excluindo o atual
            var outrosJogos = _context.Jogos
                .Where(j => j.JogoId != jogoId)
                .OrderBy(_ => Guid.NewGuid())
                .Take(4)
                .ToList();

            return PartialView("_OutrosJogos", outrosJogos);
        }





    }
}
