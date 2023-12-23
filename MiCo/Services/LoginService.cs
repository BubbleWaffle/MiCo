using MiCo.Data;
using MiCo.Helpers;
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

        /* Check if user exist and create session */
        public ResultHelper LoginUser(string? emailOrLogin, string? password)
        {
            /* Validation */
            if (string.IsNullOrWhiteSpace(emailOrLogin) || string.IsNullOrWhiteSpace(password))
                return new ResultHelper(false, "Fill all fields!");

            var user = _context.users.FirstOrDefault(u => u.email == emailOrLogin || u.login == emailOrLogin);

            /* If email or login and password are correct create session */
            if (user != null && VerifyPassword(password, user.password))
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

        /* Logout user and clear session */
        public bool LogoutUser()
        {
            _contextAccessor.HttpContext?.Session.Clear();
            return true;
        }

        /* Password validation method */
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
