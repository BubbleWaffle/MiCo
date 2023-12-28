using MiCo.Data;
using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public class ProfileContentService
    {
        private readonly MiCoDbContext _context;

        public ProfileContentService(MiCoDbContext context)
        {
            _context = context;
        }

        /* Fill profile with data */
        public Task<ProfileContentViewModel> ProfileContent(string? login)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            /* If user doesn't exist return empty view */
            if (user == null)
            {
                return Task.FromResult(new ProfileContentViewModel());
            }

            string pfp_url = user.pfp ?? "../content/default/pfp_default.svg";

            /* Create Profile object */
            var profileContentViewModel = new ProfileContentViewModel
            {
                nickname = user.nickname,
                login = user.login,
                creation_date = user.creation_date.DateTime,
                pfp = pfp_url,
                role = user.role
            };

            return Task.FromResult(profileContentViewModel);
        }
    }
}
