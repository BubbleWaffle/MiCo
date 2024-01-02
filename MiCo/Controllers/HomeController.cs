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
        /// Method used to render home page with loaded threads and top users
        /// </summary>
        /// <param name="search">Key word used for search</param>
        /// <param name="sort_option">Sort option (Latest or Hot)</param>
        /// <returns>Home view with loaded threads and top users</returns>
        public async Task<IActionResult> Index([FromQuery] string search,[FromQuery] string sort_option)
        {
            var result = new HomeViewModel();
            result._listOfThreads = await _homeService.HomeThreads(search, sort_option);
            result._topUsers = await _homeService.HomeWOF(3);

            return View(result);
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
