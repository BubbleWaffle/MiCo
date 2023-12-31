using MiCo.Data;
using MiCo.Models;
using MiCo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MiCo.Services
{
    public class HomeService : IHomeService
    {
        private readonly MiCoDbContext _context;

        public HomeService(MiCoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method used to load threads to Home view
        /// </summary>
        /// <param name="search">String entered to searchbar</param>
        /// <param name="sort_option">Sort option</param>
        /// <returns>List of threads to controller</returns>
        public async Task<List<Threads>> HomeThreads(string search, string sort_option)
        {
            var threadsQuery = _context.threads
                .Include(t => t.author)
                .Include(t => t.thread_tags!)
                    .ThenInclude(tt => tt.tag)
                .Where(t => t.id_reply == null && t.id_OG_thread == null && !t.deleted && t.author.status != -1 && t.author.status != 1);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                threadsQuery = threadsQuery
                    .Where(t => t.title.ToLower().Contains(search) ||
                                t.thread_tags!.Any(tag => tag.tag.tag.ToLower().Contains(search)));
            }

            var threadsList = await threadsQuery
                .OrderByDescending(t => t.creation_date)
                .ToListAsync();

            foreach (var thread in threadsList)
            {
                var image = await _context.images
                    .Where(i => i.id_which_thread == thread.id)
                    .FirstOrDefaultAsync();

                if (image != null) thread.thread_images = new List<Images> { image };
            }

            return threadsList;
        }

        /// <summary>
        /// Method used to load TOP 3 active users
        /// </summary>
        /// <param name="no_3">Size of list</param>
        /// <returns>Returns list of TOP 3 users</returns>
        public async Task<List<Users>> HomeWOF(int no_3)
        {
            var topUsers = await _context.users
                .Include(u => u.user_threads)
                .Where(u => u.status != -1 && u.status != 1 && u.user_threads!.Count > 0)
                .OrderByDescending(u => u.user_threads!.Count(t => !t.deleted))
                .Take(no_3)
                .ToListAsync();

            return topUsers;
        }
    }
}
