using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities
{
    public class Tools
    {
        /// <summary>
        /// Method used to check if email is valid
        /// </summary>
        /// <param name="email">String passing email</param>
        /// <returns>True if matches else false</returns>
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^(?!\.)[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        /// <summary>
        /// Method used to check if login is valid
        /// </summary>
        /// <param name="login">String passing login</param>
        /// <returns>True if matches else false</returns>
        public static bool IsValidLoginOrNickname(string login)
        {
            return Regex.IsMatch(login, "^[a-zA-Z0-9]+$");
        }

        /// <summary>
        /// Method used to check if password is valid
        /// </summary>
        /// <param name="password">String passing password</param>
        /// <returns>True if valid else false</returns>
        public static bool IsValidPassword(string password)
        {
            return password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        /// <summary>
        /// Method used to hash password
        /// </summary>
        /// <param name="password">String passing password to hash</param>
        /// <returns>Hashed password to string</returns>
        public static string HashPassword(string password)
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

        /// <summary>
        /// Method used  to verify password
        /// </summary>
        /// <param name="enteredPassword">Password entered during login process</param>
        /// <param name="storedHashedPassword">Password saved in database</param>
        /// <returns>False if passwords don't match else true</returns>
        public static bool VerifyPassword(string enteredPassword, string storedHashedPassword)
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

        /// <summary>
        /// Method used to check if title is valid
        /// </summary>
        /// <param name="title">String passing title</param>
        /// <returns>True if valid else false</returns>
        public static bool IsValidTitle(string title)
        {
            return Regex.IsMatch(title, @"[@#*/\\;]");
        }

        /// <summary>
        /// Method used to check if tags are valid
        /// </summary>
        /// <param name="tags">String passing tags</param>
        /// <returns>True if valid else false</returns>
        public static bool IsValidTags(string tags)
        {
            return Regex.IsMatch(tags, @"^[a-zA-Z0-9]+$");
        }
    }
}
