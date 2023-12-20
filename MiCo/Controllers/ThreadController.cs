using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class ThreadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create() 
        {
            return View();
        }
    }
}
