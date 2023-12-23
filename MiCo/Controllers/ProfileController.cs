using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        /* return view of specific profile */
        [HttpGet("{login}")]
        public async Task<IActionResult> Index([FromRoute(Name = "login")] string login)
        {
            var result = await _profileService.Profile(login, this);

            /* If profile doesn't exist go to home page */
            if (result.Profile == null)
                return RedirectToAction("Index", "Home");

            return View(result);
        }
    }
}
