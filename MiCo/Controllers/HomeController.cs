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

        /// <summary>
        /// Method used to render home page with loaded threads
        /// </summary>
        /// <param name="search">Key word used for search</param>
        /// <param name="sort_option">Sort option (Latest or Hot)</param>
        /// <returns>Home view with loaded threads</returns>
        public async Task<IActionResult> Index(string search, string sort_option)
        {
            var model = new HomeViewModel();
            model = await _homeService.HomeContent(search, sort_option, model);

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
