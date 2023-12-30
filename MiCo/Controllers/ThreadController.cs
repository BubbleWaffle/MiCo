using MiCo.Data;
using MiCo.Models.ViewModels;
using MiCo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiCo.Controllers
{
    public class ThreadController : Controller
    {
        private readonly MiCoDbContext _context;
        private readonly IThreadService _threadService;

        public ThreadController(MiCoDbContext context, IThreadService threadService)
        {
            _context = context;
            _threadService = threadService;
        }

        [HttpGet("/Thread/{id}")]
        public async Task<IActionResult> Index([FromRoute(Name = "id")] int id)
        {
            var thread = _context.threads.FirstOrDefault(t => t.id == id);

            if (thread == null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// Method used to render create view
        /// </summary>
        /// <returns>Create view or redirect to Home view</returns>
        public IActionResult Create() 
        {
            if (!HttpContext.Session.TryGetValue("UserId", out _))
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// Method used to create thread
        /// </summary>
        /// <param name="model">View model passing thread data to service</param>
        /// <returns>Create view with error or redirect to created thread</returns>
        [HttpPost]
        public async Task<IActionResult> Create(ThreadCreateViewModel model)
        {
            if (ModelState.IsValid) 
            {
                var result = await _threadService.ThreadCreate(HttpContext.Session.GetInt32("UserId"), model);

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
