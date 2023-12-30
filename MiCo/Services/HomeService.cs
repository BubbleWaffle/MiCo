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
            var threads = await _context.threads
                .Include(t => t.author)
                .Include(t => t.thread_tags!)
                    .ThenInclude(tt => tt.tag)
                .Where(t => t.id_reply == null && t.id_OG_thread == null && !t.deleted)
                .OrderByDescending(t => t.creation_date)
                .ToListAsync();

            foreach (var thread in threads)
            {
                var image = await _context.images
                    .Where(i => i.id_which_thread == thread.id)
                    .FirstOrDefaultAsync();

                if (image != null)
                {
                    thread.thread_images = new List<Images> { image };
                }
            }

            model._threads = threads;

            return model;
        }
    }
}
