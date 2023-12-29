using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace MiCo.Services
{
    public class LoginService
    {
        private readonly MiCoDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public LoginService(MiCoDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
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
            if (user != null && VerifyPassword(model.password, user.password) && user.status != -1 && user.status != 1)
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

        /// <summary>
        /// Method used  to verify password
        /// </summary>
        /// <param name="enteredPassword">Password entered during login process</param>
        /// <param name="storedHashedPassword">Password saved in database</param>
        /// <returns>False if passwords don't match else true</returns>
        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            byte[] hashWithSaltBytes = Convert.FromBase64String(storedHashedPassword);

            byte[] salt = new byte[16];
            Array.Copy(hashWithSaltBytes, 0, salt, 0, 16);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashWithSaltBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }
    }
}
