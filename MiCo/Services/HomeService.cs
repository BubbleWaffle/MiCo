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

        public async Task<HomeViewModel> HomeContent(string search, string sort_option, HomeViewModel model)
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

            var threads = await threadsQuery
                .OrderByDescending(t => t.creation_date)
                .ToListAsync();

            foreach (var thread in threads)
            {
                var image = await _context.images
                    .Where(i => i.id_which_thread == thread.id)
                    .FirstOrDefaultAsync();

                if (image != null) thread.thread_images = new List<Images> { image };
            }

            model._threads = threads;

            return model;
        }
    }
}
