using MiCo.Data;
using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class JusticeController : Controller
    {
        private readonly MiCoDbContext _context;
        private readonly IJusticeService _justiceService;


        public JusticeController(MiCoDbContext context, IJusticeService justiceService)
        {
            _context = context;
            _justiceService = justiceService;
        }

        /// <summary>
        /// Method used to render main justice view
        /// </summary>
        /// <returns>Justice view or redirect to Home view</returns>
        [Route("/Justice/Index")]
        public async Task<IActionResult> Index()
        {
            if (!HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("Role") != 1 )
                return RedirectToAction("Index", "Home");

            var result = await _justiceService.JusticeContent();

            return View(result);
        }

        /// <summary>
        /// Method used to render specific user ban view
        /// </summary>
        /// <param name="login">User account name passing by URL</param>
        /// <returns>Specific user ban view or redirect to Home view</returns>
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

        /// <summary>
        /// Method used to ban specific user
        /// </summary>
        /// <param name="login">User account name passing by URL</param>
        /// <param name="model">View model passing ban data to service</param>
        /// <returns>Specific user ban view with error, success or redirect to Home view</returns>
        [HttpPost("/Justice/Ban={login}")]
        public async Task<IActionResult> Ban([FromRoute(Name = "login")] string login, BanViewModel model)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") != 1)
                return RedirectToAction("Index", "Home");

            var result = await _justiceService.JusticeBan(user, HttpContext.Session.GetInt32("UserId"), model);

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

            return View(model);
        }

        /// <summary>
        /// Method used to render specific user save view
        /// </summary>
        /// <param name="id">User account id passing by URL</param>
        /// <returns>Specific user save view or redirect to Home view</returns>
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

        /// <summary>
        /// Method used to cancel specific report
        /// </summary>
        /// <param name="id">User account id passing by URL</param>
        /// <param name="model">View model passing... something to do nothing</param>
        /// <returns>Returns only Justice main view</returns>
        [HttpPost("/Justice/Save={id}")]
        public async Task<IActionResult> Save([FromRoute(Name = "id")] int id, SaveViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _justiceService.JusticeSave(id);

                if (result.RHsuccess)
                {
                    return RedirectToAction("Index", "Justice");
                }
                else
                {
                    ViewBag.error = result.RHmessage;
                }
            }
            return View(model);
        }

        /// <summary>
        /// Method used to render specific user unban view
        /// </summary>
        /// <param name="login">User account name passing by URL</param>
        /// <returns>Specific user unban view or redirect to Home view</returns>
        [HttpGet("/Justice/Unban={login}")]
        public IActionResult Unban([FromRoute(Name = "login")] string login)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") != 1)
                return RedirectToAction("Index", "Home");

            ViewBag.name = user.login;
            ViewBag.nickname = user.nickname;

            return View();
        }

        /// <summary>
        /// Method used to unban specific user
        /// </summary>
        /// <param name="login">User account name passing by URL</param>
        /// <param name="model">View model passing... something to do nothing</param>
        /// <returns>Returns only Justice main view</returns>
        [HttpPost("/Justice/Unban={login}")]
        public async Task<IActionResult> Unban([FromRoute(Name = "login")] string login, UnbanViewModel model)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") != 1)
                return RedirectToAction("Index", "Home");

            var result = await _justiceService.JusticeUnban(user.id);

            if (result.RHsuccess)
            {
                return RedirectToAction("Index", "Justice");
            }
            else
            {
                ViewBag.error = result.RHmessage;
            }

            return View(model);
        }
    }
}
