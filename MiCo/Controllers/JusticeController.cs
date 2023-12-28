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
        private readonly JusticeContentService _contentService;
        private readonly BanService _banService;
        private readonly SaveService _saveService;

        public JusticeController(MiCoDbContext context, JusticeContentService contentService, BanService banService, SaveService saveService)
        {
            _context = context;
            _contentService = contentService;
            _banService = banService;
            _saveService = saveService;
        }

        [Route("/Justice/Index")]
        public async Task<IActionResult> Index()
        {
            if (!HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("Role") != 1 )
                return RedirectToAction("Index", "Home");

            var result = await _contentService.JusticeContent();

            return View(result);
        }

        /* Specific user to ban */
        [HttpGet("/Justice/Ban={login}")]
        public IActionResult Ban([FromRoute(Name = "login")] string login)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") != 1)
                return RedirectToAction("Index", "Home");

            ViewBag.name = user.login;
            ViewBag.nickname = user.nickname;

            return View();
        }

        [HttpPost("/Justice/Ban={login}")]
        public async Task<IActionResult> Ban([FromRoute(Name = "login")] string login, BanViewModel model)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

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

        /* Specific user to save */
        [HttpGet("/Justice/Save={id}")]
        public IActionResult Save([FromRoute(Name = "id")] int id)
        {
            var report = _context.reports.FirstOrDefault(r => r.id == id);

            if (report == null)
                return RedirectToAction("Index", "Home");

            var user = _context.users.FirstOrDefault(u => u.id == report.id_reported_user);

            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") != 1)
                return RedirectToAction("Index", "Home");

            ViewBag.name = user.login;
            ViewBag.nickname = user.nickname;
            ViewBag.id = report.id;

            return View();
        }

        [HttpPost("/Justice/Save={id}")]
        public async Task<IActionResult> Save([FromRoute(Name = "id")] int id, SaveViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _saveService.JusticeSave(id);

                if (result.RHsuccess)
                {
                    return RedirectToAction("Index", "Justice");
                }
            }
            return View(model);
        }
    }
}
