using MiCo.Data;
using MiCo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MiCo.Services
{
    public class JusticeContentService
    {
        private readonly MiCoDbContext _context;

        public JusticeContentService(MiCoDbContext context) 
        {
            _context = context;
        }

        /// <summary>
        /// Method used to load content to justice page
        /// </summary>
        /// <returns>JusticeViewModel with data</returns>
        public async Task<JusticeViewModel> JusticeContent()
        {
            var justiceViewModel = new JusticeViewModel
            {
                _reports = await _context.reports
                    .Include(r => r.reported_user)
                    .Include(r => r.reporting_user)
                    .OrderBy(r => r.report_date)
                    .ToListAsync(),

                _bans = await _context.bans
                    .Include(b => b.banned_user)
                    .Include(b => b.moderator)
                    .OrderBy(b => b.ban_date)
                    .ToListAsync()
            };

            return justiceViewModel;
        }
    }
}
