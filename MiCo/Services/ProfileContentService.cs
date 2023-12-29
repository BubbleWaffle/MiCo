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

        /// <summary>
        /// Method used to load content to specific profile
        /// </summary>
        /// <param name="login">User account name passing by URL</param>
        /// <returns>ProfileContentViewModel with data</returns>
        public Task<ProfileContentViewModel> ProfileContent(string? login)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (user == null)
                return Task.FromResult(new ProfileContentViewModel());

            string pfp_url = user.pfp ?? "../content/default/pfp_default.svg";

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
