using MiCo.Controllers;
using MiCo.Data;
using MiCo.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace MiCo.Services
{
    public class ProfileService
    {
        private readonly MiCoDbContext _context;

        public ProfileService(MiCoDbContext context)
        {
            _context = context;
        }

        /* Fill profile with data */
        public Task<FullProfileViewModel> Profile(string? login, ProfileController controller)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            /* If user doesn't exist return empty view */
            if (user == null)
            {
                return Task.FromResult(new FullProfileViewModel());
            }

            string pfp_url = user.pfp ?? "../content/default/pfp_default.svg";

            /* Create Profile object */
            var profileViewModel = new ProfileViewModel
            {
                nickname = user.nickname,
                login = user.login,
                creation_date = user.creation_date.DateTime,
                pfp = pfp_url,
                role = user.role
            };

            /* Create full profile object */
            var fullProfileViewModel = new FullProfileViewModel
            {
                Profile = profileViewModel
            };

            return Task.FromResult(fullProfileViewModel);
        }
    }
}
