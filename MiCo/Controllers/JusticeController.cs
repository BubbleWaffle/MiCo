﻿using MiCo.Data;
using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class JusticeController : Controller
    {
        private readonly MiCoDbContext _context;
        private readonly JusticeContentService _justiceContentService;
        private readonly BanService _banService;
        private readonly SaveService _saveService;
        private readonly UnbanService _unbanService;

        /// <summary>
        /// Justice controller with methods used to ban, cancel reports or unban users
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="justiceContentService">Load justice content service</param>
        /// <param name="banService">Ban service</param>
        /// <param name="saveService">Cancel report service</param>
        /// <param name="unbanService">Unban service</param>
        public JusticeController(MiCoDbContext context, JusticeContentService justiceContentService, BanService banService, SaveService saveService, UnbanService unbanService)
        {
            _context = context;
            _justiceContentService = justiceContentService;
            _banService = banService;
            _saveService = saveService;
            _unbanService = unbanService;
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

            var result = await _justiceContentService.JusticeContent();

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

            var result = await _banService.JusticeBan(user, HttpContext.Session.GetInt32("UserId"), model);

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

        /* Specific user to unban */
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

        [HttpPost("/Justice/Unban={login}")]
        public async Task<IActionResult> Unban([FromRoute(Name = "login")] string login, UnbanViewModel model)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") != 1)
                return RedirectToAction("Index", "Home");

            var result = await _unbanService.JusticeUnban(user.id);

            if (result.RHsuccess)
            {
                return RedirectToAction("Index", "Justice");
            }

            return View(model);
        }
    }
}
