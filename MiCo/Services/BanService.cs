using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models;

namespace MiCo.Services
{
    public class BanService
    {
        private readonly MiCoDbContext _context;

        public BanService(MiCoDbContext context)
        {
            _context = context;
        }

        public async Task<ResultHelper> JusticeBan(Users banned_user, int? id_moderator, string reason, DateTimeOffset? ban_until)
        {
            int non_nullable_id = id_moderator ?? default(int); //Convert nullable int value to non-nullable

            if (string.IsNullOrWhiteSpace(reason))
                return new ResultHelper(false, "You have to enter reason!");

            if (ban_until <= DateTimeOffset.Now)
                return new ResultHelper(false, "Invalid date!");

            var existing_ban = _context.bans.FirstOrDefault(b => b.id_banned_user == banned_user.id && b.ban_until > DateTimeOffset.Now);

            if (existing_ban != null)
                return new ResultHelper(false, "User is already banned!");

            /* Creat report object */
            var newBan = new Bans
            {
                id_banned_user = banned_user.id,
                id_moderator = non_nullable_id,
                reason = reason,
                ban_date = DateTimeOffset.Now,
                ban_until = ban_until
            };

            banned_user.status = 1;

            _context.bans.Add(newBan);
            _context.users.Update(banned_user);
            await _context.SaveChangesAsync();

            return new ResultHelper(true, "User has been banned!");
        }
    }
}