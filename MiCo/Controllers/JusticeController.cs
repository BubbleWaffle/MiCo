using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class JusticeController : Controller
    {
        [Route("/Justice/Index")]
        public IActionResult Index()
        {
            if (!HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("Role") != 1 )
                return RedirectToAction("Index", "Home");

            return View();
        }
    }
}
