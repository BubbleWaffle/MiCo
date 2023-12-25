using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileContentService _profileContentService;
        private readonly ProfileEditService _profileEditService;

        public ProfileController(ProfileContentService profileContentService, ProfileEditService profileEditService)
        {
            _profileContentService = profileContentService;
            _profileEditService = profileEditService;
        }

        /* return view of specific profile */
        [HttpGet("{login}")]
        public async Task<IActionResult> Index([FromRoute(Name = "login")] string login)
        {
            var result = await _profileContentService.ProfileContent(login);

            /* If profile doesn't exist go to home page */
            if (result == null)
                return RedirectToAction("Index", "Home");

            return View(result);
        }

        /* Edit access action */
        public IActionResult Edit()
        {
            /* If logged in block access */
            if (!HttpContext.Session.TryGetValue("UserId", out _))
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _profileEditService.EditProfile(HttpContext.Session.GetInt32("UserId"), model.nickname,
                    model.login, model.old_password, model.new_password, model.confirm_password);
                if (result.RHsuccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", result.RHmessage);
                }
            }
            return View(model);
        }
    }
}
