using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.Services;


namespace MoonstoneTCC.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ICarteiraService _carteiraService;

        private readonly ICepFreteService _frete; // <-- campo correto, com underscore

        public PedidoController(
            IPedidoRepository pedidoRepository,
            CarrinhoCompra carrinhoCompra,
            UserManager<IdentityUser> userManager,
            AppDbContext context,
            ICarteiraService carteiraService,
            ICepFreteService frete) // <-- injeta pelo construtor
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoCompra = carrinhoCompra;
            _userManager = userManager;
            _context = context;
            _carteiraService = carteiraService;
            _frete = frete; // <-- atribui
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);

            // Endereço padrão (se houver)
            var endereco = await _context.EnderecosEntrega
                .FirstOrDefaultAsync(e => e.UserId == user.Id && e.EnderecoPadrao);

            if (endereco != null)
            {
                if (string.IsNullOrWhiteSpace(endereco.Email))
                {
                    endereco.Email = user.Email;
                    _context.EnderecosEntrega.Update(endereco);
                    await _context.SaveChangesAsync();
                }
                ViewBag.EnderecoSalvo = endereco;
            }

            // Cartão salvo
            var cartao = await _context.CartoesCredito
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.CartaoPadrao);
            if (cartao != null) ViewBag.CartaoSalvo = cartao;

            // Saldo da carteira
            var carteira = await _carteiraService.ObterOuCriarAsync(user.Id);
            ViewBag.CarteiraSaldo = carteira.Saldo;

            // ===== FRETE (local) =====
            var itens = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItems = itens;

            var subtotal = _carrinhoCompra.GetCarrinhoCompraTotal();
            var qtdItens = itens.Sum(i => i.Quantidade);

            // Usa CEP da query (?cep=) se vier; cai para o CEP do endereço padrão
            string cepUsado = Request.Query["cep"].ToString();
            if (string.IsNullOrWhiteSpace(cepUsado))
                cepUsado = endereco?.Cep;

            if (!string.IsNullOrWhiteSpace(cepUsado))
            {
                var cot = _frete.Calcular(cepUsado, subtotal, qtdItens);
                ViewBag.FreteValor = cot.Valor;
                ViewBag.FretePrazoDias = cot.PrazoDias;
                ViewBag.FreteRegiao = cot.Regiao;
                ViewBag.CepUsado = cepUsado;
            }

            ViewBag.Email = user.Email;
            TempData["MensagemSucesso"] = "Jogada iniciada! Agora é só finalizar o pagamento.";
            return View();
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(Pedido pedido)
        {
            var user = await _userManager.GetUserAsync(User);

            // Email do usuário
            pedido.Email = user.Email;

            // Endereço padrão
            var endereco = await _context.EnderecosEntrega
                .FirstOrDefaultAsync(e => e.UserId == user.Id && e.EnderecoPadrao);

            if (endereco != null)
            {
                if (string.IsNullOrWhiteSpace(endereco.Email))
                {
                    endereco.Email = user.Email;
                    _context.EnderecosEntrega.Update(endereco);
                    await _context.SaveChangesAsync();
                }
                ViewBag.EnderecoSalvo = endereco;
            }

            // Cartão salvo (se houver)
            var cartao = await _context.CartoesCredito
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.CartaoPadrao);
            if (cartao != null) ViewBag.CartaoSalvo = cartao;

            // Forma de pagamento vinda da View: "Cartao" ou "Carteira"
            var formaPagamento = Request.Form["formaPagamento"].ToString();

            // Carrinho
            int totalItensPedido = 0;
            decimal subtotal = 0m;

            var items = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItems = items;

            if (_carrinhoCompra.CarrinhoCompraItems.Count == 0)
            {
                ModelState.AddModelError("", "Seu carrinho está vazio...");
                return View(pedido);
            }

            foreach (var item in items)
            {
                totalItensPedido += item.Quantidade;
                subtotal += (item.PrecoUnitario * item.Quantidade);
            }

            pedido.TotalItensPedido = totalItensPedido;
            pedido.UserId = user.Id;

            // ===== FRETE (local) =====
            // 1) tenta pegar um campo oculto da View (name="cepCalculo"), senão usa pedido.Cep, senão endereço padrão
            string cepDestino = Request.Form["cepCalculo"].ToString();
            if (string.IsNullOrWhiteSpace(cepDestino)) cepDestino = pedido.Cep;
            if (string.IsNullOrWhiteSpace(cepDestino)) cepDestino = endereco?.Cep;

            var cot = _frete.Calcular(cepDestino, subtotal, totalItensPedido);
            pedido.ValorFrete = cot.Valor;                 // <- persistido no banco (adicione no modelo/migration)
            pedido.PrazoEntregaDias = cot.PrazoDias;       // <- idem
            pedido.PedidoTotal = subtotal + cot.Valor;     // <- total FINAL com frete

            // Guarda na ViewBag caso precise reexibir a view por erro
            ViewBag.FreteValor = cot.Valor;
            ViewBag.FretePrazoDias = cot.PrazoDias;
            ViewBag.FreteRegiao = cot.Regiao;
            ViewBag.CepUsado = cepDestino;

            // Salva endereço do pedido como padrão se ainda não existir
            var jaTemEndereco = await _context.EnderecosEntrega
                .AnyAsync(e => e.UserId == user.Id && e.Endereco1 == pedido.Endereco1);

            if (!jaTemEndereco)
            {
                var novoEndereco = new EnderecoEntrega
                {
                    UserId = user.Id,
                    Nome = pedido.Nome,
                    Sobrenome = pedido.Sobrenome,
                    Endereco1 = pedido.Endereco1,
                    Endereco2 = pedido.Endereco2,
                    Cidade = pedido.Cidade,
                    Estado = pedido.Estado,
                    Cep = pedido.Cep,
                    Telefone = pedido.Telefone,
                    EnderecoPadrao = true,
                    Email = user.Email
                };

                _context.EnderecosEntrega.Add(novoEndereco);
                await _context.SaveChangesAsync();
            }

            if (ModelState.IsValid)
            {
                // ===== PAGAMENTO =====
                if (string.Equals(formaPagamento, "Carteira", StringComparison.OrdinalIgnoreCase))
                {
                    var carteira = await _carteiraService.ObterOuCriarAsync(user.Id);
                    if (carteira.Saldo < pedido.PedidoTotal) // agora já inclui frete
                    {
                        ModelState.AddModelError("", "Saldo insuficiente na carteira.");
                        return View(pedido);
                    }

                    _pedidoRepository.CriarPedido(pedido);
                    await _carteiraService.DebitarParaPedidoAsync(user.Id, pedido.PedidoTotal, pedido.PedidoId);
                }
                else
                {
                    if (cartao != null)
                    {
                        var codigoDigitado = Request.Form["codigoVerificacaoCartao"].ToString();
                        if (codigoDigitado != cartao.CodigoVerificacao)
                        {
                            ModelState.AddModelError("", "Código de verificação do cartão inválido.");
                            return View(pedido);
                        }
                    }
                    else
                    {
                        var numeroCartao = Request.Form["numeroCartao"].ToString();
                        var validade = Request.Form["validadeCartao"].ToString();
                        var nomeTitular = Request.Form["nomeTitular"].ToString();
                        var codigoVerificacao = Request.Form["cvvCartao"].ToString();

                        bool jaTemCartao = await _context.CartoesCredito.AnyAsync(c => c.UserId == user.Id);

                        var novoCartao = new CartaoCredito
                        {
                            UserId = user.Id,
                            NumeroParcial = numeroCartao[^4..],
                            Validade = validade,
                            NomeTitular = nomeTitular,
                            CodigoVerificacao = codigoVerificacao,
                            CartaoPadrao = !jaTemCartao
                        };

                        _context.CartoesCredito.Add(novoCartao);
                        await _context.SaveChangesAsync();
                    }

                    _pedidoRepository.CriarPedido(pedido);
                }

                // Recarrega o pedido completo (com itens)
                var pedidoCompleto = await _context.Pedidos
                    .Include(p => p.PedidoItens).ThenInclude(d => d.Jogo)
                    .FirstOrDefaultAsync(p => p.PedidoId == pedido.PedidoId);

                ViewBag.CheckoutCompletoMensagem = "Obrigado pelo seu pedido!";
                ViewBag.TotalPedido = pedido.PedidoTotal; // agora inclui frete
                ViewBag.ValorFrete = pedido.ValorFrete;
                ViewBag.PrazoEntregaDias = pedido.PrazoEntregaDias;
                _carrinhoCompra.LimparCarrinho();

                return View("~/Views/Pedido/CheckoutCompleto.cshtml", pedidoCompleto);
            }

            TempData["MensagemSucesso"] = "Jogada iniciada! Agora é só finalizar o pagamento.";
            return View(pedido);
        }



        [Authorize]
        public async Task<IActionResult> CompreNovamente(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens)
                .ThenInclude(i => i.Jogo)
                .FirstOrDefaultAsync(p => p.PedidoId == id && p.UserId == user.Id);

            if (pedido == null)
            {
                TempData["MensagemErro"] = "Pedido não encontrado.";
                return RedirectToAction("Index");
            }

            // Limpa o carrinho atual
            _carrinhoCompra.LimparCarrinho();

            // Adiciona os mesmos itens ao carrinho
            foreach (var item in pedido.PedidoItens)
            {
                _carrinhoCompra.AdicionarAoCarrinho(item.Jogo, item.Quantidade);
            }

            TempData["MensagemSucesso"] = "Jogos adicionados novamente ao carrinho!";
            return RedirectToAction("Checkout");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Cancelar(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens)
                    .ThenInclude(i => i.Jogo)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null || pedido.StatusPedido == "Cancelado")
            {
                return NotFound();
            }

            return View(pedido);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarConfirmado(int id, string motivo)
        {
            var user = await _userManager.GetUserAsync(User);

            // Carrega pedido + itens (precisa para repor estoque)
            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens)
                .FirstOrDefaultAsync(p => p.PedidoId == id && p.UserId == user.Id);

            if (pedido == null)
                return NotFound();

            if (pedido.StatusPedido == "Cancelado")
            {
                TempData["MensagemErro"] = "Este pedido já foi cancelado.";
                return RedirectToAction("Index", "MeusPedidos");
            }

            // Atualiza status/data/motivo
            pedido.StatusPedido = "Cancelado";
            pedido.MotivoCancelamento = motivo;
            pedido.DataCancelamento = DateTime.Now;

            // Repor estoque dos itens
            foreach (var item in pedido.PedidoItens)
            {
                if (item.JogoId.HasValue)
                {
                    var jogo = await _context.Jogos.FindAsync(item.JogoId.Value);
                    if (jogo != null) jogo.QuantidadeEstoque += item.Quantidade;
                }
                if (item.AcessorioId.HasValue)
                {
                    var acessorio = await _context.Acessorios.FindAsync(item.AcessorioId.Value);
                    if (acessorio != null) acessorio.QuantidadeEstoque += item.Quantidade;
                }
            }

            // Registra o cancelamento (se tiver a entidade)
            var cancelamento = new CancelamentoPedido
            {
                PedidoId = id,
                UsuarioId = user.Id,
                Motivo = motivo,
                DataCancelamento = DateTime.Now
            };
            _context.CancelamentosPedidos.Add(cancelamento);

            await _context.SaveChangesAsync();

            // 💼 Estorno para a carteira (sempre volta como saldo) com idempotência
            if (!await _carteiraService.JaEstornadoAsync(pedido.PedidoId))
            {
                await _carteiraService.EstornarPedidoAsync(user.Id, pedido.PedidoTotal, pedido.PedidoId);
            }

            TempData["MensagemSucesso"] = "Seu pedido foi cancelado, o estoque foi atualizado e o valor foi creditado na sua carteira.";
            return RedirectToAction("Index", "MeusPedidos");
        }


    }
}
