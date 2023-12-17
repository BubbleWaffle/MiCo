using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly RegistrationService _registrationService;

        public AuthorizationController(RegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        public IActionResult Login()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _registrationService.RegisterUser(model.email, model.login, model.password, model.confirm_password);

                if (result.RHsuccess)
                {
                    TempData["SuccessMessage"] = result.RHmessage;
                    return RedirectToAction("Login");
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
