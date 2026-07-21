using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Extensions;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.ViewModels;
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
            // =================== 1. MAIS VENDIDOS E PROMOÇÕES (GLOBAL) ===================
            // Garantindo lista sem duplicatas e na ordem exata de vendas
            var maisVendidosBrutos = _jogoRepository.GetJogosMaisComprados(10)?.ToList() ?? new List<Jogo>();

            var maisVendidos = maisVendidosBrutos
                .GroupBy(j => j.JogoId)
                .Select(g => g.First())
                .ToList();

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

            // Mapeador auxiliar para DTO dos cards de descoberta
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
            };

            // Listas auxiliares para usuários logados
            var favDTO = new List<DiscoverCardDTO>();
            var compradosDTO = new List<DiscoverCardDTO>();
            var compreNovamenteDTO = new List<DiscoverCardDTO>();
            var vistosRecentementeDTO = new List<DiscoverCardDTO>();
            var oQueVoceQuerDTO = new List<DiscoverCardDTO>();
            var interessaDTO = new List<DiscoverCardDTO>();

            // =================== 2. PROCESSAMENTO DO USUÁRIO LOGADO ===================
            if (User.Identity.IsAuthenticated)
            {
                var usuario = await _userManager.GetUserAsync(User);
                if (usuario != null)
                {
                    var userId = usuario.Id;
                    var email = usuario.Email;
                    modelo.NomeUsuario = usuario.UserName;

                    // Favoritos
                    var favoritosUser = await _context.Favoritos
                        .Where(f => f.UsuarioId == userId)
                        .Include(f => f.Jogo).ThenInclude(j => j.Categoria)
                        .OrderByDescending(f => f.Id)
                        .Select(f => f.Jogo)
                        .Distinct()
                        .Take(12)
                        .ToListAsync();

                    modelo.FavoritosDoUsuario = favoritosUser;
                    favDTO = favoritosUser.Select(Map).ToList();

                    // Continuar vendo (Histórico)
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
                    oQueVoceQuerDTO = continuarVendo.Select(Map).ToList();

                    // Vistos e não comprados
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

                    // Recomendados por categoria
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

                    interessaDTO = modelo.RecomendadosPorCategoria.Select(Map).ToList();

                    // Comprados
                    var comprados = await _context.PedidoDetalhes
                        .Include(p => p.Pedido)
                        .Where(p => p.Pedido.UserId == userId && p.JogoId != null)
                        .Select(p => p.Jogo!)
                        .Distinct()
                        .ToListAsync();

                    compradosDTO = comprados.Select(Map).ToList();
                    compreNovamenteDTO = comprados.Skip(1).Select(Map).ToList();

                    // Vistos recentemente
                    var vistosRecentemente = await _context.Jogos
                        .Where(j => ultimosVistosIds.Contains(j.JogoId))
                        .ToListAsync();

                    vistosRecentementeDTO = vistosRecentemente.Select(Map).ToList();

                    ViewBag.ListasDoUsuario = await _context.ListasJogos
                        .Where(l => l.UsuarioId == userId)
                        .ToListAsync();
                    ViewBag.ListasMarcadas = new List<int>();
                }
            }

            // =================== 3. CONSTRUÇÃO DINÂMICA DOS PAINÉIS (DISCOVERY ROW) ===================
            var allPanels = new List<DiscoverPanelVM>();

            // Painel Global 1: Mais Vendidos (Sempre exibido se houver jogos)
            var maisVendidosDTO = modelo.MaisVendidosSemana.Select(Map).ToList();
            if (maisVendidosDTO.Any())
            {
                allPanels.Add(new DiscoverPanelVM { Key = "mais-vendidos", Titulo = "Mais vendidos", Itens = maisVendidosDTO });
            }

            // Painéis Condicionais (Aparecem progressivamente conforme ações do usuário)
            if (favDTO.Any())
            {
                allPanels.Add(new DiscoverPanelVM { Key = "compre-favorito", Titulo = "Compre seu favorito", Itens = favDTO });
            }

            if (compradosDTO.Any())
            {
                allPanels.Add(new DiscoverPanelVM { Key = "repita-mercado", Titulo = "Repita o mercado", Itens = compradosDTO });
            }

            if (oQueVoceQuerDTO.Any())
            {
                allPanels.Add(new DiscoverPanelVM { Key = "o-que-voce-quer", Titulo = "O que você quer", Itens = oQueVoceQuerDTO });
            }

            if (compreNovamenteDTO.Any())
            {
                allPanels.Add(new DiscoverPanelVM { Key = "compre-novamente", Titulo = "Compre novamente", Itens = compreNovamenteDTO });
            }

            if (vistosRecentementeDTO.Any())
            {
                allPanels.Add(new DiscoverPanelVM { Key = "visto-recentemente", Titulo = "Visto recentemente", Itens = vistosRecentementeDTO });
            }

            if (interessaDTO.Any())
            {
                allPanels.Add(new DiscoverPanelVM { Key = "tambem-te-interessa", Titulo = "Também te interessa", Itens = interessaDTO });
            }

            // Painel Global 2: Ofertas / Promoções (Ótimo para novos usuários)
            var promoDTO = modelo.EmPromocao.Select(Map).ToList();
            if (promoDTO.Any())
            {
                allPanels.Add(new DiscoverPanelVM { Key = "em-promocao", Titulo = "Em promoção", Itens = promoDTO });
            }

            // Utilitários
            if (User.Identity.IsAuthenticated)
            {
                allPanels.Add(new DiscoverPanelVM
                {
                    Key = "minha-carteira",
                    Titulo = "Minha Carteira",
                    Itens = new List<DiscoverCardDTO> {
                        new DiscoverCardDTO { IsUtility = true, Nome = "Minha Carteira", ImagemUrl = "/img/ui/carteira.png", Link = "/Carteira", UtilitySubtitle = "Saldo e extrato" }
                    }
                });

                allPanels.Add(new DiscoverPanelVM
                {
                    Key = "meios-pagamento",
                    Titulo = "Meios de pagamento",
                    Itens = new List<DiscoverCardDTO> {
                        new DiscoverCardDTO { IsUtility = true, Nome = "Meios de pagamento", ImagemUrl = "/img/ui/cartao.png", Link = "/Cartao", UtilitySubtitle = "Seus cartões salvos" }
                    }
                });
            }

            // Monta as linhas de descoberta apenas com os painéis válidos
            if (allPanels.Any())
            {
                modelo.DiscoverRows = new List<DiscoverRowVM>
                {
                    new DiscoverRowVM
                    {
                        RowId = "row-descoberta-unica",
                        Colunas = allPanels
                    }
                };
            }

            return View(modelo);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}