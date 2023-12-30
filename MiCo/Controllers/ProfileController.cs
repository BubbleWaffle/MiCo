using MiCo.Data;
using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class ProfileController : Controller
    {
        private readonly MiCoDbContext _context;
        private readonly IProfileService _profileService;
        public ProfileController(MiCoDbContext context, IProfileService profileService)
        {
            _context = context;
            _profileService = profileService;
        }

        /// <summary>
        /// Method used to render specific profile view
        /// </summary>
        /// <param name="login">User account name passing by URL</param>
        /// <returns>Profile view with content or redirect to Home view</returns>
        [HttpGet("{login}")]
        public async Task<IActionResult> Index([FromRoute(Name = "login")] string login)
        {
            var result = await _profileService.ProfileContent(login);

            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (result.login == null || user == null || user.status == -1 || user.status == 1)
                return RedirectToAction("Index", "Home");

            return View(result);
        }

        /// <summary>
        /// Method used to render edit view
        /// </summary>
        /// <returns>Edit view or redirect to Home view</returns>
        public IActionResult Edit()
        {
            if (!HttpContext.Session.TryGetValue("UserId", out _))
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// Method used to edit profile
        /// </summary>
        /// <param name="model">View model passing edited data to service</param>
        /// <returns>Edit view with error or redirect to specific profile view with success</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                /* Convert string to bool */
                bool deletePfp = !string.IsNullOrEmpty(model.delete_pfp) && model.delete_pfp.ToLower() == "true";

                var result = await _profileService.ProfileEdit(HttpContext.Session.GetInt32("UserId"), model, deletePfp);

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

        /// <summary>
        /// Method used to render specific profile report view
        /// </summary>
        /// <param name="login">User account name passing by URL</param>
        /// <returns>Report specific profile view or redirect to Home view</returns>
        [HttpGet("Report/{login}")]
        public IActionResult Report([FromRoute(Name = "login")] string login)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") == 1 || user.status == -1 || user.status == 1)
                return RedirectToAction("Index", "Home");

            ViewBag.name = user.login;
            ViewBag.nickname = user.nickname;

            return View();
        }

        /// <summary>
        /// Method used to report specific profile
        /// </summary>
        /// <param name="login">User account name passing by URL</param>
        /// <param name="model">View model passing report data to service</param>
        /// <returns>Report specific profile view with error, success or redirect to Home view</returns>
        [HttpPost("Report/{login}")]
        public async Task<IActionResult> Report([FromRoute(Name = "login")] string login, ProfileReportViewModel model)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (user == null || !HttpContext.Session.TryGetValue("UserId", out _) || HttpContext.Session.GetInt32("UserId") == user.id || HttpContext.Session.GetInt32("Role") == 1)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                var result = await _profileService.ProfileReport(user.id, HttpContext.Session.GetInt32("UserId"), model);

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

        /// <summary>
        /// Method used to render delete profile view
        /// </summary>
        /// <returns>Delete profile view or redirect to Home view</returns>
        public IActionResult Delete()
        {
            /* If not logged in block access */
            if (!HttpContext.Session.TryGetValue("UserId", out _))
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// Method used to delete profile
        /// </summary>
        /// <param name="model">View model passing delete data to service</param>
        /// <returns>Delete profile view with error or redirect to Home view with success</returns>
        [HttpPost]
        public async Task<IActionResult> Delete(ProfileDeleteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _profileService.ProfileDelete(HttpContext.Session.GetInt32("UserId"), model);

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