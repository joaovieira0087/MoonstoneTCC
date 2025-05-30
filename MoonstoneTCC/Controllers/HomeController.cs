using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.ViewModels;
using System.Diagnostics;

namespace MoonstoneTCC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJogoRepository _jogoRepository;

        public HomeController(IJogoRepository jogoRepository)
        {
            _jogoRepository = jogoRepository;
        }

        public IActionResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                JogosPreferidos = _jogoRepository.JogosPreferidos
            };
            return View(homeViewModel);
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


    }
}

