using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
