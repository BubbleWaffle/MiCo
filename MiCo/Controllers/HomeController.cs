using MiCo.Models;
using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MiCo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index(string search, string sort_option)
        {
            var model = new HomeViewModel(); // Tworzymy nowy obiekt HomeViewModel
            model = await _homeService.HomeContent(search, sort_option, model); // Używamy metody HomeContent, aby uzupełnić dane

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
