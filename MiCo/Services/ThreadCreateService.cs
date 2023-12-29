using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models;
using MiCo.Models.ViewModels;
using System.Text.RegularExpressions;

namespace MiCo.Services
{
    public class ThreadCreateService
    {
        private readonly MiCoDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ThreadCreateService(MiCoDbContext context, IWebHostEnvironment hostEnvironment)
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
                if (string.IsNullOrWhiteSpace(model.title) && !IsValidTitle(model.title))
                    return new ResultHelper(false, "Invalid title!");

                if (model.title.Length >= 75)
                    return new ResultHelper(false, "Title is too long (MAX 75 characters)!");

                if (string.IsNullOrWhiteSpace(model.description))
                    return new ResultHelper(false, "Description can't be empty!");

                if (model.description.Length > 500)
                    return new ResultHelper(false, "Description is too long (MAX 500 characters)!");

                if (model.files != null && model.files.Count > 3)
                    return new ResultHelper(false, "You can send only 3 files!");

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
                            image = $"../content/thread/{threadFolderName}/{uniqueFileName}"
                        };

                        _context.images.Add(image);
                        await _context.SaveChangesAsync();

                        var threadImage = new ThreadImages
                        {
                            id_thread = thread.id,
                            id_image = image.id
                        };

                        _context.thread_images.Add(threadImage);
                        await _context.SaveChangesAsync();

                        counter++;
                    }
                }

                return new ResultHelper(true, "Thread created successfully!");
            }

            return new ResultHelper(false, "You can't do that!");
        }

        /// <summary>
        /// Method used to check if title is valid
        /// </summary>
        /// <param name="title">String passing title</param>
        /// <returns>True if valid else false</returns>
        public static bool IsValidTitle(string title)
        {
            return Regex.IsMatch(title, @"[@#*/\\;]");
        }
    }
}
