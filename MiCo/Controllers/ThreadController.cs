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

            var user_id = HttpContext.Session.GetInt32("UserId");

            var result = await _threadService.OGThreadContent(id, user_id);
            result._replies = await _threadService.RepliesContent(id, user_id);

            return View(result);
        }

        /// <summary>
        /// Method used to liked or dislike threads
        /// </summary>
        /// <param name="id">Id OG Thread</param>
        /// <param name="model">View model passing like or dislike data to service</param>
        /// <returns>Redirect to main Thread view</returns>
        [HttpPost("/Thread/ThreadNo={id}")]
        public async Task<IActionResult> Index([FromRoute(Name = "id")] int id, ThreadViewModel model)
        {
            if (model.id_liked_thread != null) await _threadService.ThreadLike(model.id_liked_thread, HttpContext.Session.GetInt32("UserId"));
            if (model.id_disliked_thread != null) await _threadService.ThreadDislike(model.id_disliked_thread, HttpContext.Session.GetInt32("UserId"));

            return RedirectToAction("Index", new { id = id });
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
            var threadExists = _context.threads.FirstOrDefault(t => t.id == id);

            if (!HttpContext.Session.TryGetValue("UserId", out _) || threadExists == null)
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

        /// <summary>
        /// Method used to render Edith thread view
        /// </summary>
        /// <param name="id">Thread id</param>
        /// <returns>Returns view with some data</returns>
        [HttpGet("/Thread/Edit={id}")]
        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int id)
        {
            var threadExists = _context.threads.FirstOrDefault(t => t.id == id);

            if (!HttpContext.Session.TryGetValue("UserId", out _) || threadExists == null || threadExists.id_author != HttpContext.Session.GetInt32("UserId"))
                return RedirectToAction("Index", "Home");

            var ThreadToEdit = await _threadService.ThreadToEdit(threadExists.id);

            return View(ThreadToEdit);
        }

        /// <summary>
        /// Method used to edit thread
        /// </summary>
        /// <param name="id">Thread id</param>
        /// <param name="model">View model passing edit data to service</param>
        /// <returns>Edit view with error or redirect to thread view</returns>
        [HttpPost("/Thread/Edit={id}")]
        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int id, ThreadEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool deleteFiles = !string.IsNullOrEmpty(model.delete_files) && model.delete_files.ToLower() == "true";

                var result = await _threadService.ThreadEdit(id, model, deleteFiles);

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
        /// Method used to render delete view
        /// </summary>
        /// <param name="id">Id thread to delete</param>
        /// <returns>Delete view or redirect to Home view</returns>
        [HttpGet("/Thread/Delete={id}")]
        public IActionResult Delete([FromRoute(Name = "id")] int id)
        {
            var threadExists = _context.threads.FirstOrDefault(t => t.id == id);

            if (!HttpContext.Session.TryGetValue("UserId", out _) || threadExists == null || (threadExists.id_author != HttpContext.Session.GetInt32("UserId") && HttpContext.Session.GetInt32("Role") != 1))
                return RedirectToAction("Index", "Home");

            var thread = new ThreadDeleteViewModel();

            if (threadExists.id_OG_thread == null)
            {
                thread.thread_id = threadExists.id;
            }
            else
            {
                thread.thread_id = threadExists.id_OG_thread ?? default(int);
            }

            return View(thread);
        }

        /// <summary>
        /// Method used to reply
        /// </summary>
        /// <param name="id">Id thread to reply</param>
        /// <param name="model">View model passing reply data to service</param>
        /// <returns>Reply view with error or redirect to thread view</returns>
        [HttpPost("/Thread/Delete={id}")]
        public async Task<IActionResult> Delete([FromRoute(Name = "id")] int id, ThreadDeleteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _threadService.ThreadDelete(id, HttpContext.Session.GetInt32("UserId"), model);

                if (result.RHsuccess && result.RHno == -1)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (result.RHsuccess && result.RHno > 0)
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
