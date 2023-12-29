using MiCo.Data;
using MiCo.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MiCo.Services
{
    public class UnbanService
    {
        private readonly MiCoDbContext _context;

        public UnbanService(MiCoDbContext context)
        {
            _context = context;
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
    }
}
