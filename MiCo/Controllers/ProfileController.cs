using MiCo.Data;
using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class ProfileController : Controller
    {
        private readonly MiCoDbContext _context;
        private readonly ProfileContentService _profileContentService;
        private readonly ProfileEditService _profileEditService;
        private readonly ProfileReportService _profileReportService;
        private readonly ProfileDeleteService _profileDeleteService;

        public ProfileController(MiCoDbContext context, ProfileContentService profileContentService, ProfileEditService profileEditService, ProfileReportService profileReportService, ProfileDeleteService profileDeleteService)
        {
            _context = context;
            _profileContentService = profileContentService;
            _profileEditService = profileEditService;
            _profileReportService = profileReportService;
            _profileDeleteService = profileDeleteService;
        }

        /* Return view of specific profile */
        [HttpGet("{login}")]
        public async Task<IActionResult> Index([FromRoute(Name = "login")] string login)
        {
            var result = await _profileContentService.ProfileContent(login);

            var user = _context.users.FirstOrDefault(u => u.login == login);

            /* If profile doesn't exist go to home page */
            if (result.login == null || user == null || user.status == -1 || user.status == 1)
                return RedirectToAction("Index", "Home");

            return View(result);
        }

        /* Edit access action */
        public IActionResult Edit()
        {
            /* If not logged in block access */
            if (!HttpContext.Session.TryGetValue("UserId", out _))
                return RedirectToAction("Index", "Home");

            return View();
        }

        /* Edit profile action */
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                /* Convert string to bool */
                bool deletePfp = !string.IsNullOrEmpty(model.delete_pfp) && model.delete_pfp.ToLower() == "true";

                var result = await _profileEditService.ProfileEdit(HttpContext.Session.GetInt32("UserId"), model.nickname,
                    model.login, model.file, deletePfp, model.old_password, model.new_password, model.confirm_password);

                if (result.RHsuccess)
                {
                    return RedirectToAction("index", HttpContext.Session.GetString("Login"));
                }
                else
                {
                    ModelState.AddModelError("", result.RHmessage);
                }
            }
            return View(model);
        }

        /* Report access action */
        [HttpGet("Report/{login}")]
        public IActionResult Report([FromRoute(Name = "login")] string login)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            /* If user doesn't exist return home page */
            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") == 1 || user.status == -1 || user.status == 1)
                return RedirectToAction("Index", "Home");

            ViewBag.name = user.login;

            return View();
        }

        /* Report profile action */
        [HttpPost("Report/{login}")]
        public async Task<IActionResult> Report([FromRoute(Name = "login")] string login, ProfileReportViewModel model)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            /* If user doesn't exist return home page */
            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") == 1)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                var result = await _profileReportService.ProfileReport(user.id, HttpContext.Session.GetInt32("UserId"), model.reason);

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

        /* Delete access action */
        public IActionResult Delete()
        {
            /* If not logged in block access */
            if (!HttpContext.Session.TryGetValue("UserId", out _))
                return RedirectToAction("Index", "Home");

            return View();
        }

        /* Delete profile action */
        [HttpPost]
        public async Task<IActionResult> Delete(ProfileDeleteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _profileDeleteService.ProfileDelete(HttpContext.Session.GetInt32("UserId"), model.password);

                if (result.RHsuccess)
                {
                    ViewBag.Success = result.RHsuccess;
                    ViewBag.SuccessMessage = result.RHmessage;
                    return RedirectToAction("Index", "Home");
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