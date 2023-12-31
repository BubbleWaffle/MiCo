using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models;
using MiCo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MiCo.Services
{
    public class JusticeService : IJusticeService
    {
        private readonly MiCoDbContext _context;

        public JusticeService(MiCoDbContext context)
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

        /// <summary>
        /// Method used to ban users
        /// </summary>
        /// <param name="banned_user">Banned user</param>
        /// <param name="id_moderator">Moderator who banned user</param>
        /// <param name="model">View model passing ban data</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> JusticeBan(Users banned_user, int? id_moderator, BanViewModel model)
        {
            int non_nullable_id = id_moderator ?? default(int); // Convert nullable int value to non-nullable

            if (string.IsNullOrWhiteSpace(model.reason))
                return new ResultHelper(false, "You have to enter reason!");

            if (model.reason.Length > 300)
                return new ResultHelper(false, "Your reason is too long!");

            if (model.ban_until <= DateTimeOffset.Now)
                return new ResultHelper(false, "Invalid date!");

            var existing_ban = _context.bans.FirstOrDefault(b => b.id_banned_user == banned_user.id);

            if (existing_ban != null)
                return new ResultHelper(false, "User is already banned!");

            var threadsToSoftDelete = _context.threads
                .Where(t => t.id_author == banned_user.id && !t.deleted);

            foreach (var thread in threadsToSoftDelete)
            {
                thread.deleted = true;
                _context.threads.Update(thread);
            }

            var reportsToDelete = _context.reports.Where(r => r.id_reported_user == banned_user.id);
            _context.reports.RemoveRange(reportsToDelete);

            var newBan = new Bans
            {
                id_banned_user = banned_user.id,
                id_moderator = non_nullable_id,
                reason = model.reason,
                ban_date = DateTimeOffset.Now,
                ban_until = model.ban_until
            };

            banned_user.status = 1;

            _context.bans.Add(newBan);
            _context.users.Update(banned_user);
            await _context.SaveChangesAsync();

            return new ResultHelper(true, "User has been banned!");
        }

        /// <summary>
        /// Method used to unban users manually
        /// </summary>
        /// <param name="id">Users id to unban</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> JusticeUnban(int id)
        {
            var user = await _context.users.FindAsync(id);

            if (user != null)
            {
                var banToRemove = await _context.bans.FirstOrDefaultAsync(b => b.id_banned_user == id);

                if (banToRemove != null)
                {
                    _context.bans.Remove(banToRemove);
                    user.status = 0;
                    _context.users.Update(user);
                    await _context.SaveChangesAsync();

                    return new ResultHelper(true, "User unbanned successfully!");
                }

                return new ResultHelper(false, "This ban doesn't exist!");
            }

            return new ResultHelper(false, "This user doesn't exist!");
        }

        /// <summary>
        /// Method used in AutoUnbanService (HostedService) to automatic unban users
        /// </summary>
        public void Unban()
        {
            var now = DateTimeOffset.Now;
            var expiredBans = _context.bans.Where(b => b.ban_until <= now).ToList();

            foreach (var ban in expiredBans)
            {
                var bannedUser = _context.users.Find(ban.id_banned_user);

                if (bannedUser != null)
                {
                    bannedUser.status = 0;
                    _context.users.Update(bannedUser);
                }

                _context.bans.Remove(ban);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Method used to cancel report by removing from the database
        /// </summary>
        /// <param name="id">User account id passing by URL</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> JusticeSave(int id)
        {
            var reportToRemove = await _context.reports.FindAsync(id);

            if (reportToRemove != null)
            {
                _context.reports.Remove(reportToRemove);

                await _context.SaveChangesAsync();

                return new ResultHelper(true, "Report deleted successfully!");
            }

            return new ResultHelper(false, "This report doesn't exist!");
        }
    }
}
