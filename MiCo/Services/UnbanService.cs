using MiCo.Data;

namespace MiCo.Services
{
    public class UnbanService
    {
        private readonly MiCoDbContext _context;

        public UnbanService(MiCoDbContext context)
        {
            _context = context;
        }

        /* Method unbanning users */
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
