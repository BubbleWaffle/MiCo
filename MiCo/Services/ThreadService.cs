using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models.ViewModels;
using MiCo.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MiCo.Services
{
    public class ThreadService : IThreadService
    {
        private readonly MiCoDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ThreadService(MiCoDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Method used to create thread
        /// </summary>
        /// <param name="id">Id currently logged user</param>
        /// <param name="model">View model passing thread data</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> ThreadCreate(int? id, ThreadCreateViewModel model)
        {
            if (model != null && id != null)
            {
                string[]? tagsArray = null;

                if (string.IsNullOrWhiteSpace(model.title) && !Utilities.Tools.IsValidTitle(model.title))
                    return new ResultHelper(false, "Invalid title!");

                if (model.title.Length >= 75)
                    return new ResultHelper(false, "Title is too long (MAX 75 characters)!");

                if (string.IsNullOrWhiteSpace(model.description))
                    return new ResultHelper(false, "Description can't be empty!");

                if (model.description.Length > 500)
                    return new ResultHelper(false, "Description is too long (MAX 500 characters)!");

                if (model.files != null && model.files.Count > 3)
                    return new ResultHelper(false, "You can send only 3 files!");

                if (!string.IsNullOrWhiteSpace(model.tags))
                {
                    tagsArray = model.tags.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (!tagsArray.All(tag => Utilities.Tools.IsValidTags(tag)))
                        return new ResultHelper(false, "Invalid tags!");

                    tagsArray = tagsArray.Select(tag => $"#{tag.Trim()}").Distinct().ToArray();

                    if (tagsArray.Length > 9)
                        return new ResultHelper(false, "Too many tags (MAX 9)!");
                }

                var thread = new Threads
                {
                    id_author = id.Value,
                    title = model.title,
                    description = model.description,
                    creation_date = DateTimeOffset.Now,
                    deleted = false
                };

                _context.threads.Add(thread);
                await _context.SaveChangesAsync();

                if (tagsArray != null)
                {
                    foreach (var tagText in tagsArray)
                    {
                        var existingTag = await _context.tags.FirstOrDefaultAsync(t => t.tag == tagText);

                        if (existingTag == null)
                        {
                            var newTag = new Tags
                            {
                                tag = tagText
                            };
                            _context.tags.Add(newTag);
                            await _context.SaveChangesAsync();

                            var threadTag = new ThreadTags
                            {
                                id_thread = thread.id,
                                id_tag = newTag.id
                            };

                            _context.thread_tags.Add(threadTag);
                        }
                        else
                        {
                            var threadTag = new ThreadTags
                            {
                                id_thread = thread.id,
                                id_tag = existingTag.id
                            };

                            _context.thread_tags.Add(threadTag);
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                if (model.files != null && model.files.Count > 0)
                {
                    string threadFolderName = thread.id.ToString();
                    string threadFolder = Path.Combine(_hostEnvironment.WebRootPath, "content", "thread", threadFolderName);

                    if (!Directory.Exists(threadFolder))
                        Directory.CreateDirectory(threadFolder);

                    int counter = 1;

                    foreach (var file in model.files)
                    {
                        string uniqueFileName = $"{counter}{Path.GetExtension(file.FileName)}";
                        string filePath = Path.Combine(threadFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        var image = new Images
                        {
                            id_which_thread = thread.id,
                            image = $"../content/thread/{threadFolderName}/{uniqueFileName}"
                        };

                        _context.images.Add(image);
                        await _context.SaveChangesAsync();

                        counter++;
                    }
                }

                return new ResultHelper(true, "Thread created successfully!", thread.id);
            }

            return new ResultHelper(false, "You can't do that!");
        }

        /// <summary>
        /// Method used to load OG thread in Thread view
        /// </summary>
        /// <param name="id">OG thread id</param>
        /// <returns>Thread with data</returns>
        public async Task<Threads> OGThreadContent(int id)
        {
            var ogThread = await _context.threads
                .Include(t => t.author)
                .Include(t => t.thread_images)
                .FirstOrDefaultAsync(t => t.id == id);

            return ogThread!;
        }

        /// <summary>
        /// Method used to load list of thread replies
        /// </summary>
        /// <param name="id">OG thread id</param>
        /// <returns>List of threads with data</returns>
        public async Task<List<Threads>> RepliesContent(int id)
        {
            var replies = await _context.threads
                .Include(t => t.author)
                .Include(t => t.thread_images)
                .Where(t => t.id_OG_thread == id)
                .OrderBy(t => t.creation_date)
                .ToListAsync();

            return replies;
        }
    }
}
