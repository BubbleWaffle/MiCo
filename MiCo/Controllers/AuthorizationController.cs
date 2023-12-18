using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class AuthorizationController : Controller
    {
        /* Add services */
        private readonly RegistrationService _registrationService;
        private readonly LoginService _loginService;

        public AuthorizationController(RegistrationService registrationService, LoginService loginService)
        {
            _registrationService = registrationService;
            _loginService = loginService;
        }

        /* Login access action */
        public IActionResult Login()
        {
            /* If logged in block access */
            if (HttpContext.Session.TryGetValue("UserId", out _))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;

            return View();
        }

        /* Logout action */
        public IActionResult Logout()
        {
            _loginService.LogoutUser();

            return RedirectToAction("Login");
        }

        /* Login action */
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _loginService.LoginUser(model.email_or_login, model.password);

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

        /* Registration access action */
        public IActionResult Registration()
        {
            if (HttpContext.Session.TryGetValue("UserId", out _))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /* Registration action */
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
