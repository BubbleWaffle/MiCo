using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MiCo.Services
{
    public class ProfileEditService
    {
        private readonly MiCoDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProfileEditService(MiCoDbContext context, IHttpContextAccessor contextAccessor, IWebHostEnvironment hostEnvironment)
        {  
            _context = context;
            _contextAccessor = contextAccessor;
            _hostEnvironment = hostEnvironment;
        }

        /* Edit existing user */
        public async Task<ResultHelper> EditProfile(int? id, string? nickname, string? login, IFormFile? file, bool delete_pfp, string? old_password, string? new_password, string? confirm_password)
        {
            var user = _context.users.FirstOrDefault(u => u.id == id);

            if (user != null)
            {
                if (string.IsNullOrWhiteSpace(old_password))
                    return new ResultHelper(false, "Please enter your current password!");

                if (!string.IsNullOrWhiteSpace(nickname) && IsValidLoginOrNickname(nickname)) user.nickname = nickname;

                if (!string.IsNullOrWhiteSpace(login))
                {
                    if (login.Length < 4 || login.Length > 14)
                        return new ResultHelper(false, "Login is too short or too long (min 4 characters, max 14 characters)!");

                    if (!IsValidLoginOrNickname(login))
                        return new ResultHelper(false, "Invalid login!");

                    if (_context.users.Any(u => u.login == login))
                        return new ResultHelper(false, "Provided login is already in use or is the same as current!");

                    user.login = login;
                }

                if (file != null && file.Length > 0)
                {
                    if (file.Length > 15728640)
                        return new ResultHelper(false, "Image is too big (MAX 15MB)!");
                    // Generuj unikalną nazwę pliku, używając id użytkownika i rozszerzenia oryginalnego pliku
                    string uniqueFileName = $"{id}.{Path.GetExtension(file.FileName)}";

                    // Uzyskaj ścieżkę do folderu, w którym będą przechowywane pliki
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "content", "pfp");

                    // Pełna ścieżka do zapisu pliku
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Zapisz plik na dysku
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Zaktualizuj ścieżkę w bazie danych (przykładowo, dodając ścieżkę do user.pfp)
                    user.pfp = $"../content/pfp/{uniqueFileName}";
                }

                if (delete_pfp) user.pfp = null;

                if (!string.IsNullOrWhiteSpace(new_password))
                {
                    if (string.IsNullOrWhiteSpace(confirm_password))
                        return new ResultHelper(false, "You have to confirm your new password!");

                    if (!IsValidPassword(new_password))
                        return new ResultHelper(false, "Password must contain special characters and numbers!");

                    if (new_password.Length < 8)
                        return new ResultHelper(false, "Password is too short (min 8 characters)!");

                    if (confirm_password != new_password)
                        return new ResultHelper(false, "New password does not match!");

                    if (old_password == new_password)
                        return new ResultHelper(false, "New password cannot be the same as the old one!");

                    if (!VerifyPassword(old_password, user.password))
                        return new ResultHelper(false, "Incorrect password!");

                    var hashedPassword = HashPassword(new_password); //Hash password

                    user.password = hashedPassword;
                }

                _context.users.Update(user);
                await _context.SaveChangesAsync();

                UpdateSession(user);

                return new ResultHelper(true, "Changes saved!");
            }

            return new ResultHelper(false, "Something went wrong!");
        }

        /* Method helping with login validation */
        private bool IsValidLoginOrNickname(string login_or_nickname)
        {
            return Regex.IsMatch(login_or_nickname, "^[a-zA-Z0-9]+$");
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

        /* Update session method */
        private void UpdateSession(Users user)
        {
            _contextAccessor.HttpContext?.Session.SetString("Nickname", user.nickname);
            _contextAccessor.HttpContext?.Session.SetString("Login", user.login);
            _contextAccessor.HttpContext?.Session.SetInt32("Role", user.role);
            if (user.pfp != null) _contextAccessor.HttpContext?.Session.SetString("PFP", user.pfp);
            else _contextAccessor.HttpContext?.Session.SetString("PFP", "../content/default/pfp_default.svg");
        }
    }
}
