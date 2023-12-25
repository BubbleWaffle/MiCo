using MiCo.Data;
using MiCo.Models;
using MiCo.Helpers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MiCo.Services
{
    public class RegistrationService
    {
        private readonly MiCoDbContext _context;

        public RegistrationService(MiCoDbContext context)
        {
            _context = context;
        }

        /* Adding new user to database */
        public async Task<ResultHelper> RegisterUser(string? email, string? login, string? password, string? confirm_password)
        {
            /* Validation */
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                return new ResultHelper(false, "Invalid email address!");

            if (_context.users.Any(u => u.email == email))
                return new ResultHelper(false, "Provided email is already in use!");

            if (string.IsNullOrWhiteSpace(login) || !IsValidLogin(login))
                return new ResultHelper(false, "Invalid login!");

            if (login.Length < 4 || login.Length > 14)
                return new ResultHelper(false, "Login is too short or too long (min 4 characters, max 14 characters)!");

            if (_context.users.Any(u => u.login == login))
                return new ResultHelper(false, "Provided login is already in use!");

            if (string.IsNullOrWhiteSpace(password) || !IsValidPassword(password))
                return new ResultHelper(false, "Password must contain special characters and numbers!");

            if (password.Length < 8)
                return new ResultHelper(false, "Password is too short (min 8 characters)!");

            if (string.IsNullOrWhiteSpace(confirm_password))
                return new ResultHelper(false, "You need to confirm password!");

            if (confirm_password != password)
                return new ResultHelper(false, "Password does not match!");

            var hashedPassword = HashPassword(password); //Hash password

            /* Creating user object */
            var newUser = new Users
            {
                nickname = login,
                login = login,
                password = hashedPassword,
                email = email,
                creation_date = DateTimeOffset.Now,
                role = 0,
                status = 0
            };

            _context.users.Add(newUser); //Add new user to database
            await _context.SaveChangesAsync();

            return new ResultHelper(true, "Your account has been registered, now you can log in!");
        }

        /* Method helping with email validation */
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^(?!\.)[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        /* Method helping with login validation */
        private bool IsValidLogin(string login)
        {
            return Regex.IsMatch(login, "^[a-zA-Z0-9]+$");
        }

        /* Method helping with password validation */
        private bool IsValidPassword(string password)
        {
            return password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        /* Method hashing password */
        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, 10000, HashAlgorithmName.SHA256);

            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashWithSalt = new byte[36];
            Array.Copy(salt, 0, hashWithSalt, 0, 16);
            Array.Copy(hash, 0, hashWithSalt, 16, 20);

            return Convert.ToBase64String(hashWithSalt);
        }
    }
}
