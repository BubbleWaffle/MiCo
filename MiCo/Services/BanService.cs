using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models;
using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public class BanService
    {
        private readonly MiCoDbContext _context;

        public BanService(MiCoDbContext context)
        {
            _context = context;
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
    }
}