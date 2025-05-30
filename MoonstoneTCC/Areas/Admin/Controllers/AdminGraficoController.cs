using MoonstoneTCC.Areas.Admin.Servicos;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Areas.Admin.Services;
using System;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminGraficoController : Controller
    {
        private readonly GraficoVendasService _graficoVendas;

        public AdminGraficoController(GraficoVendasService graficoVendas)
        {
            _graficoVendas = graficoVendas ?? throw
                new ArgumentNullException(nameof(graficoVendas));
        }

        /// <summary>
        /// Retorna dados do gráfico de vendas para um intervalo padrão ou personalizado
        /// </summary>
        public JsonResult VendasJogos(int dias = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            var jogosVendasTotais = new List<JogoGrafico>();

            if (startDate.HasValue && endDate.HasValue)
            {
                // Verificar intervalo válido
                if (startDate > endDate)
                {
                    return Json(new
                    {
                        success = false,
                        message = "A data de início não pode ser maior que a data de término."
                    });
                }

                jogosVendasTotais = _graficoVendas.GetVendasJogosPersonalizado(startDate.Value, endDate.Value);
            }
            else
            {
                jogosVendasTotais = _graficoVendas.GetVendasJogos(dias);
            }

            if (!jogosVendasTotais.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "Nenhum dado encontrado para o intervalo especificado."
                });
            }

            return Json(new
            {
                success = true,
                data = jogosVendasTotais
            });

        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VendasMensal()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VendasSemanal()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VendasTrimestral()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VendasTotais()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VendasCustomizadas()
        {
            return View();
        }
    }
}
