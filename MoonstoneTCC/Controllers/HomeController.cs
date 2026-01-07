using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.ViewModels;
using MoonstoneTCC.Extensions;
using System.Diagnostics;


namespace MoonstoneTCC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(IJogoRepository jogoRepository, UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _jogoRepository = jogoRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // =================== Mais vendidos + ranking ===================
            var maisVendidos = _jogoRepository.GetJogosMaisComprados(10).ToList();

            var rankingDict = maisVendidos
                .Select((j, idx) => new { j.JogoId, Pos = idx + 1 })
                .ToDictionary(x => x.JogoId, x => x.Pos);

            var modelo = new HomePersonalizadaViewModel
            {
                MaisVendidosSemana = maisVendidos,
                RankingMaisVendidos = rankingDict,
                EmPromocao = await _context.Jogos
                    .Where(j => j.PrecoPromocional != null)
                    .Take(12)
                    .ToListAsync(),
                UsuarioLogado = User.Identity.IsAuthenticated
            };

            // Visitante (não logado): já retorna a home com base pública
            if (!User.Identity.IsAuthenticated)
                return View(modelo);

            // =================== Usuário logado ===================
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return View(modelo); // fallback seguro

            var userId = usuario.Id;
            var email = usuario.Email;
            modelo.NomeUsuario = usuario.UserName;

            // Favoritos (até 12)
            var favoritosUser = await _context.Favoritos
                .Where(f => f.UsuarioId == userId)
                .Include(f => f.Jogo).ThenInclude(j => j.Categoria)
                .OrderByDescending(f => f.Id)
                .Select(f => f.Jogo)
                .Distinct()
                .Take(12)
                .ToListAsync();

            modelo.FavoritosDoUsuario = favoritosUser;

            // Continuar vendo (histórico mais recente – 12)
            var continuarVendo = await _context.HistoricoVisualizacoes
                .Where(h => h.UsuarioId == userId)
                .OrderByDescending(h => h.DataVisualizacao)
                .Select(h => h.JogoId)
                .Distinct()
                .Take(20)
                .Join(_context.Jogos.Include(j => j.Categoria),
                      id => id, j => j.JogoId, (id, j) => j)
                .Take(12)
                .ToListAsync();

            modelo.ContinuarVendo = continuarVendo;

            // Você viu e ainda não comprou (até 8)
            var ultimosVistosIds = await _context.HistoricoVisualizacoes
                .Where(h => h.UsuarioId == userId)
                .OrderByDescending(h => h.DataVisualizacao)
                .Select(h => h.JogoId)
                .Distinct()
                .Take(30)
                .ToListAsync();

            var viuNaoComprou = new List<Jogo>();
            foreach (var jid in ultimosVistosIds)
            {
                if (!await _context.UsuarioComprouJogoPorEmailAsync(email, jid))
                {
                    var jogo = await _context.Jogos
                        .Include(j => j.Categoria)
                        .FirstOrDefaultAsync(j => j.JogoId == jid);

                    if (jogo != null) viuNaoComprou.Add(jogo);
                    if (viuNaoComprou.Count >= 8) break;
                }
            }
            modelo.ViuMasNaoComprou = viuNaoComprou;

            // Recomendados por categoria (mistura favoritos + histórico)
            var categoriasBase = favoritosUser.Select(j => j.Categoria?.CategoriaNome)
                .Concat(continuarVendo.Select(j => j.Categoria?.CategoriaNome))
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .Take(3)
                .ToList();

            if (categoriasBase.Count == 0 && ultimosVistosIds.Count > 0)
            {
                categoriasBase = await _context.Jogos
                    .Where(j => ultimosVistosIds.Contains(j.JogoId))
                    .Select(j => j.Categoria.CategoriaNome)
                    .Distinct()
                    .Take(3)
                    .ToListAsync();
            }

            var recomendados = new List<Jogo>();
            foreach (var cat in categoriasBase)
            {
                var daCategoria = await _context.Jogos
                    .Include(j => j.Categoria)
                    .Where(j => j.Categoria.CategoriaNome == cat)
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(6)
                    .ToListAsync();

                recomendados.AddRange(daCategoria);
                if (recomendados.Count >= 18) break;
            }

            // Evita duplicatas nas recomendações
            var jaExibidos = new HashSet<int>(
                modelo.FavoritosDoUsuario.Select(j => j.JogoId)
                    .Concat(modelo.ContinuarVendo.Select(j => j.JogoId))
                    .Concat(modelo.ViuMasNaoComprou.Select(j => j.JogoId))
            );

            modelo.RecomendadosPorCategoria = recomendados
                .Where(j => !jaExibidos.Contains(j.JogoId))
                .GroupBy(j => j.JogoId)
                .Select(g => g.First())
                .Take(18)
                .ToList();

            // Extras (listas do usuário)
            ViewBag.ListasDoUsuario = await _context.ListasJogos
                .Where(l => l.UsuarioId == userId)
                .ToListAsync();
            ViewBag.ListasMarcadas = new List<int>();

            // =================== DISCOVERY ROW: 1 fileira (5 visíveis) ===================

            // Map Jogo -> DiscoverCardDTO
            DiscoverCardDTO Map(Jogo j) => new DiscoverCardDTO
            {
                JogoId = j.JogoId,
                Nome = j.Nome,
                ImagemUrl = !string.IsNullOrWhiteSpace(j.ImagemUrl)
                    ? j.ImagemUrl
                    : (!string.IsNullOrWhiteSpace(j.ImagemThumbnailUrl)
                        ? j.ImagemThumbnailUrl
                        : "/img/placeholder.jpg"),
                Preco = j.Preco,
                PrecoPromocional = j.PrecoPromocional
                // Link pode ficar null: a Partial usa /Jogo/Details?jogoId={id}
            };

            // Comprados (distintos)
            var comprados = await _context.PedidoDetalhes
                .Include(p => p.Pedido)
                .Where(p => p.Pedido.UserId == userId && p.JogoId != null)
                .Select(p => p.Jogo!)
                .Distinct()
                .ToListAsync();

            var compradosDTO = comprados.Select(Map).ToList();
            var compreNovamenteDTO = comprados.Skip(3).Select(Map).ToList();

            // Vistos recentemente (embaralha)
            var vistosRecentementeIds = await _context.HistoricoVisualizacoes
                .Where(h => h.UsuarioId == userId)
                .OrderByDescending(h => h.DataVisualizacao)
                .Select(h => h.JogoId)
                .Distinct()
                .Take(40)
                .ToListAsync();

            var rnd = new Random();
            var vistosRecentemente = await _context.Jogos
                .Where(j => vistosRecentementeIds.Contains(j.JogoId))
                .ToListAsync();
            vistosRecentemente = vistosRecentemente.OrderBy(_ => rnd.Next()).ToList();
            var vistosRecentementeDTO = vistosRecentemente.Select(Map).ToList();

            // Outras listas para os painéis
            var interessaDTO = modelo.RecomendadosPorCategoria.Select(Map).ToList();
            var favDTO = modelo.FavoritosDoUsuario.Select(Map).ToList();
            var oQueVoceQuerDTO = modelo.ContinuarVendo.Select(Map).ToList();

            // Utilitários
            var utilFavoritos = new DiscoverCardDTO
            {
                IsUtility = true,
                Nome = "Veja seus favoritos",
                ImagemUrl = "/img/ui/favoritos.png",
                Link = "/Favorito",
                UtilitySubtitle = "Gerencie e compre de novo"
            };
            var utilPagto = new DiscoverCardDTO
            {
                IsUtility = true,
                Nome = "Meios de pagamento",
                ImagemUrl = "/img/ui/cartao.png",
                Link = "/Cartao",
                UtilitySubtitle = "Seus cartões salvos"
            };
            var utilCarteira = new DiscoverCardDTO
            {
                IsUtility = true,
                Nome = "Minha Carteira",
                ImagemUrl = "/img/ui/carteira.png",
                Link = "/Carteira",
                UtilitySubtitle = "Saldo e extrato"
            };

            // Todos os painéis em UMA única row (a partial mostra 5 e navega com setas)
            var allPanels = new List<DiscoverPanelVM>
    {
        new DiscoverPanelVM { Key="compre-favorito",     Titulo="Compre seu favorito",   Itens = favDTO },
        new DiscoverPanelVM { Key="repita-mercado",      Titulo="Repita o mercado",      Itens = compradosDTO },
        new DiscoverPanelVM { Key="o-que-voce-quer",     Titulo="O que você quer",       Itens = oQueVoceQuerDTO },
        new DiscoverPanelVM { Key="compre-novamente",    Titulo="Compre novamente",      Itens = compreNovamenteDTO },
        new DiscoverPanelVM { Key="visto-recentemente",  Titulo="Visto recentemente",    Itens = vistosRecentementeDTO },
        new DiscoverPanelVM { Key="tambem-te-interessa", Titulo="Também te interessa",   Itens = interessaDTO },
        new DiscoverPanelVM { Key="veja-favoritos",      Titulo="Veja seus favoritos",   Itens = new List<DiscoverCardDTO>{ utilFavoritos } },
        new DiscoverPanelVM { Key="meios-pagamento",     Titulo="Meios de pagamento",    Itens = new List<DiscoverCardDTO>{ utilPagto } },
        new DiscoverPanelVM { Key="minha-carteira",      Titulo="Minha Carteira",        Itens = new List<DiscoverCardDTO>{ utilCarteira } },
        new DiscoverPanelVM { Key="mais-vendidos",       Titulo="Mais vendidos",         Itens = modelo.MaisVendidosSemana.Select(Map).ToList() }
    };

            modelo.DiscoverRows = new List<DiscoverRowVM>
    {
        new DiscoverRowVM
        {
            RowId  = "row-descoberta-unica",
            Colunas = allPanels
        }
    };

            // =================== Final ===================
            return View(modelo);
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Terms()
        {
            return View();  // procura por Views/Home/Terms.cshtml
        }

        [HttpGet]
        public IActionResult ShippingPolicy()
        {
            // Vai renderizar Views/Home/ShippingPolicy.cshtml
            return View();
        }

        [HttpGet]
        public IActionResult ReturnPolicy()
        {
            return View(); // Procura por Views/Home/ReturnPolicy.cshtml
        }

        [HttpGet]
        public IActionResult PrivacyPolicy()
        {
            return View();   // Vai procurar Views/Home/PrivacyPolicy.cshtml
        }

        [HttpGet]
        public IActionResult FAQ()
        {
            return View(); // Vai procurar por Views/Home/FAQ.cshtml
        }

        [HttpGet]
        public IActionResult Sobre()
        {
            return View(); // Vai renderizar Views/Home/Sobre.cshtml
        }

        [HttpGet]
        public IActionResult NossoTime()
        {
            return View(); // Vai procurar Views/Home/NossoTime.cshtml
        }

        [HttpGet]
        public IActionResult TrabalheConosco()
        {
            return View(); // Vai renderizar Views/Home/TrabalheConosco.cshtml
        }


        [HttpPost]
        public async Task<IActionResult> EnviarCurriculo(string Nome, string Email, string Mensagem, IFormFile Curriculo)
        {
            if (Curriculo != null && Curriculo.Length > 0)
            {
                var caminho = Path.Combine("wwwroot", "uploads", Curriculo.FileName);
                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await Curriculo.CopyToAsync(stream);
                }

                // Aqui você pode salvar os dados no banco ou enviar por email
                TempData["Mensagem"] = "Currículo enviado com sucesso!";
                return RedirectToAction("TrabalheConosco");
            }

            TempData["Erro"] = "Erro ao enviar currículo. Verifique o arquivo.";
            return View("TrabalheConosco");
        }





    }
}

