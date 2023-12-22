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
            string pfp_url;
            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (user == null)
            {
                controller.Response.Redirect("/");
                return Task.FromResult(new FullProfileViewModel());
            }

            if (user.pfp == null) pfp_url = "https://via.placeholder.com/40";
            else pfp_url = user.pfp;

            var profileViewModel = new ProfileViewModel
            {
                nickname = user.nickname,
                login = user.login,
                creation_date = user.creation_date,
                pfp = pfp_url,
                role = user.role
            };

            var fullProfileViewModel = new FullProfileViewModel
            {
                Profile = profileViewModel
            };

            return Task.FromResult(fullProfileViewModel);
        }
    }
}
