using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly RegistrationService _registrationService;
        private readonly LoginService _loginService;

        /// <summary>
        /// Authorization controller with methods used to login and registrate users
        /// </summary>
        /// <param name="registrationService">Registration service</param>
        /// <param name="loginService">Login service</param>
        public AuthorizationController(RegistrationService registrationService, LoginService loginService)
        {
            _registrationService = registrationService;
            _loginService = loginService;
        }

        /// <summary>
        /// Method used to render login view
        /// </summary>
        /// <returns>Login view or redirect to Home view</returns>
        public IActionResult Login()
        {
            if (HttpContext.Session.TryGetValue("UserId", out _))
                return RedirectToAction("Index", "Home");

            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;

            return View();
        }

        /// <summary>
        /// Method used to logout user
        /// </summary>
        /// <returns>Redirect to login view</returns>
        public IActionResult Logout()
        {
            _loginService.LogoutUser();

            return RedirectToAction("Login");
        }

        /// <summary>
        /// Method used to login user
        /// </summary>
        /// <param name="model">View model passing login data to service</param>
        /// <returns>Login view with error or redirect to Home view with success</returns>
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _loginService.LoginUser(model);

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

        /// <summary>
        /// Method used to render registration view
        /// </summary>
        /// <returns>Registration view or redirect to Home view</returns>
        public IActionResult Registration()
        {
            if (HttpContext.Session.TryGetValue("UserId", out _))
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// Method used to registrate user
        /// </summary>
        /// <param name="model">View model passing registration data to service</param>
        /// <returns>Registration view with error or redirect to Login view with success</returns>
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _registrationService.RegisterUser(model);

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
