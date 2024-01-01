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

        /// <summary>
        /// Method used to render Thread view
        /// </summary>
        /// <param name="id">Id OG Thread</param>
        /// <returns>Create view with data</returns>
        [HttpGet("/Thread/ThreadNo={id}")]
        public async Task<IActionResult> Index([FromRoute(Name = "id")] int id)
        {
            var thread = _context.threads.FirstOrDefault(t => t.id == id);

            if (thread == null || thread.deleted || thread.id_reply != null || thread.id_OG_thread != null)
                return RedirectToAction("Index", "Home");

            var result = new ThreadViewModel();
            result._OGThread = thread;
            result._replies = await _threadService.RepliesContent(id);

            return View(result);
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
                    return RedirectToAction("Index", new { id = result.RHno });
                }
                else
                {
                    ModelState.AddModelError("", result.RHmessage);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Method used to render reply view
        /// </summary>
        /// <param name="id">Id thread to reply</param>
        /// <returns>Reply view or redirect to Home view</returns>
        [HttpGet("/Thread/Reply={id}")]
        public IActionResult Reply([FromRoute(Name = "id")] int id) 
        { 
            var thredExists = _context.threads.FirstOrDefault(t => t.id == id);

            if (!HttpContext.Session.TryGetValue("UserId", out _) || thredExists == null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// Method used to reply
        /// </summary>
        /// <param name="id">Id thread to reply</param>
        /// <param name="model">View model passing reply data to service</param>
        /// <returns>Reply view with error or redirect to thread view</returns>
        [HttpPost("/Thread/Reply={id}")]
        public async Task<IActionResult> Reply([FromRoute(Name = "id")] int id, ThreadReplyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _threadService.ThreadReply(id, HttpContext.Session.GetInt32("UserId"), model);

                if (result.RHsuccess)
                {
                    return RedirectToAction("Index", new { id = result.RHno });
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
