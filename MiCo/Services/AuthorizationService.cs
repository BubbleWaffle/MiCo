using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models.ViewModels;
using MiCo.Models;

namespace MiCo.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly MiCoDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthorizationService(MiCoDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Method used to register user
        /// </summary>
        /// <param name="model">View model passing registration data</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> RegisterUser(RegistrationViewModel model)
        {
            /* Validation */
            if (string.IsNullOrWhiteSpace(model.email) || !Utilities.Tools.IsValidEmail(model.email))
                return new ResultHelper(false, "Invalid email address!");

            if (_context.users.Any(u => u.email == model.email))
                return new ResultHelper(false, "Provided email is already in use!");

            if (string.IsNullOrWhiteSpace(model.login) || !Utilities.Tools.IsValidLoginOrNickname(model.login))
                return new ResultHelper(false, "Invalid login!");

            if (model.login.Length < 4 || model.login.Length > 14)
                return new ResultHelper(false, "Login is too short or too long (min 4 characters, max 14 characters)!");

            if (_context.users.Any(u => u.login == model.login))
                return new ResultHelper(false, "Provided login is already in use!");

            if (string.IsNullOrWhiteSpace(model.password) || !Utilities.Tools.IsValidPassword(model.password))
                return new ResultHelper(false, "Password must contain special characters and numbers!");

            if (model.password.Length < 8)
                return new ResultHelper(false, "Password is too short (min 8 characters)!");

            if (string.IsNullOrWhiteSpace(model.confirm_password))
                return new ResultHelper(false, "You need to confirm password!");

            if (model.confirm_password != model.password)
                return new ResultHelper(false, "Password does not match!");

            var hashedPassword = Utilities.Tools.HashPassword(model.password);

            var newUser = new Users
            {
                nickname = model.login,
                login = model.login,
                password = hashedPassword,
                email = model.email,
                creation_date = DateTimeOffset.Now,
                role = 0,
                status = 0
            };

            _context.users.Add(newUser);
            await _context.SaveChangesAsync();

            return new ResultHelper(true, "Your account has been registered, now you can log in!");
        }

        /// <summary>
        /// Method used to login user
        /// </summary>
        /// <param name="model">View model passing login data</param>
        /// <returns>Helper reporting success or error</returns>
        public ResultHelper LoginUser(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.email_or_login) || string.IsNullOrWhiteSpace(model.password))
                return new ResultHelper(false, "Fill all fields!");

            var user = _context.users.FirstOrDefault(u => u.email == model.email_or_login || u.login == model.email_or_login);

            /* Create session */
            if (user != null && Utilities.Tools.VerifyPassword(model.password, user.password) && user.status != -1 && user.status != 1)
            {
                _contextAccessor.HttpContext?.Session.SetInt32("UserId", user.id);
                _contextAccessor.HttpContext?.Session.SetString("Nickname", user.nickname);
                _contextAccessor.HttpContext?.Session.SetString("Login", user.login);
                _contextAccessor.HttpContext?.Session.SetInt32("Role", user.role);
                if (user.pfp != null) _contextAccessor.HttpContext?.Session.SetString("PFP", user.pfp);
                else _contextAccessor.HttpContext?.Session.SetString("PFP", "../content/default/pfp_default.svg");
                return new ResultHelper(true, "Login successful!");
            }

            return new ResultHelper(false, "Incorrect login or password!");
        }

        /// <summary>
        /// Method used to logout user and clear session
        /// </summary>
        /// <returns>True</returns>
        public bool LogoutUser()
        {
            _contextAccessor.HttpContext?.Session.Clear();
            return true;
        }
    }
}
