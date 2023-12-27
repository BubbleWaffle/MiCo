using MiCo.Data;
using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiCo.Controllers
{
    public class JusticeController : Controller
    {
        private readonly MiCoDbContext _context;
        private readonly BanService _banService;

        public JusticeController(MiCoDbContext context, BanService banService)
        {
            _context = context;
            _banService = banService;
        }

        [Route("/Justice/Index")]
        public IActionResult Index()
        {
            if (!HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("Role") != 1 )
                return RedirectToAction("Index", "Home");

            return View();
        }

        /* Specific user to ban */
        [HttpGet("/Justice/Ban={login}")]
        public IActionResult Ban([FromRoute(Name = "login")] string login)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            /* If user doesn't exist return home page */
            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") != 1)
                return RedirectToAction("Index", "Home");

            ViewBag.name = user.login;

            return View();
        }

        [HttpPost("/Justice/Ban={login}")]
        public async Task<IActionResult> Ban([FromRoute(Name = "login")] string login, BanViewModel model)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            /* If user doesn't exist return home page */
            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") != 1)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                var result = await _banService.JusticeBan(user, HttpContext.Session.GetInt32("UserId"), model.reason, model.ban_until);

                if (result.RHsuccess)
                {
                    ViewBag.Success = result.RHsuccess;
                    ViewBag.SuccessMessage = result.RHmessage;
                }
                else
                {
                    ViewBag.Success = result.RHsuccess;
                    ViewBag.SuccessMessage = result.RHmessage;
                }
            }
            return View(model);
        }
    }
}
