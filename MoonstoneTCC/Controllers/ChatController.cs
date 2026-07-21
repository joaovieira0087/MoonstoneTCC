//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Google.GenAI;

//namespace MoonstoneTCC.Controllers
//{
//    public class ChatController : Controller
//    {
//        private readonly string _apiKey;

//        public ChatController(IConfiguration config)
//        {
//            // Pega a chave do appsettings.json
//            _apiKey = config["Gemini:ApiKey"];
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> EnviarMensagem([FromBody] ChatInput input)
//        {
//            if (string.IsNullOrEmpty(input?.Mensagem))
//                return BadRequest("Mensagem vazia");

//            try
//            {
//                // 1. O NOME CORRETO NA 1.6.0 É GeminiClient
//                // Tente instanciar assim para forçar a busca no pacote
//                var client = new Google.GenAI.GeminiClient(_apiKey);

//                string dadosDoSite = "Promoções: Elden Ring (R$ 150), FIFA 26 (R$ 200). Suporte: suporte@moonstone.com";
//                string promptContexto = $"Você é o assistente do site MoonstoneTCC. " +
//                                        $"Dados atuais: {dadosDoSite}. " +
//                                        $"Pergunta: {input.Mensagem}";

//                // 2. Chamada da IA
//                var response = await client.Models.GenerateContentAsync("gemini-1.5-flash", promptContexto);

//                return Json(new { resposta = response.Text });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { resposta = "Erro: " + ex.Message });
//            }
//        }
//    }

//    public class ChatInput
//    {
//        public string Mensagem { get; set; }
//    }
//}