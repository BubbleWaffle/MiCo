using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models;
using MiCo.Models.ViewModels;
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
        public async Task<ResultHelper> ProfileEdit(int? id, ProfileEditViewModel model, bool delete_pfp)
        {
            var user = _context.users.FirstOrDefault(u => u.id == id);

            if (user != null)
            {
                if (string.IsNullOrWhiteSpace(model.old_password))
                    return new ResultHelper(false, "Please enter your current password!");

                if (!string.IsNullOrWhiteSpace(model.nickname) && IsValidLoginOrNickname(model.nickname)) user.nickname = model.nickname;

                if (!string.IsNullOrWhiteSpace(model.login))
                {
                    if (model.login.Length < 4 || model.login.Length > 14)
                        return new ResultHelper(false, "Login is too short or too long (min 4 characters, max 14 characters)!");

                    if (!IsValidLoginOrNickname(model.login))
                        return new ResultHelper(false, "Invalid login!");

                    if (_context.users.Any(u => u.login == model.login))
                        return new ResultHelper(false, "Provided login is already in use or is the same as current!");

                    user.login = model.login;
                }

                if (model.file != null && model.file.Length > 0)
                {
                    if (model.file.Length > 15728640)
                        return new ResultHelper(false, "Image is too big (MAX 15MB)!");

                    string uniqueFileName = $"{id}.{Path.GetExtension(model.file.FileName)}"; //Use user id as file name

                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "content", "pfp"); //Path to folder with avatars

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName); //Full path with file name

                    /* Save file */
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.file.CopyToAsync(fileStream);
                    }

                    user.pfp = $"../content/pfp/{uniqueFileName}"; //Update path in database
                }

                if (delete_pfp) user.pfp = null;

                if (!string.IsNullOrWhiteSpace(model.new_password))
                {
                    if (string.IsNullOrWhiteSpace(model.confirm_password))
                        return new ResultHelper(false, "You have to confirm your new password!");

                    if (!IsValidPassword(model.new_password))
                        return new ResultHelper(false, "Password must contain special characters and numbers!");

                    if (model.new_password.Length < 8)
                        return new ResultHelper(false, "Password is too short (min 8 characters)!");

                    if (model.confirm_password != model.new_password)
                        return new ResultHelper(false, "New password does not match!");

                    if (model.old_password == model.new_password)
                        return new ResultHelper(false, "New password cannot be the same as the old one!");

                    if (!VerifyPassword(model.old_password, user.password))
                        return new ResultHelper(false, "Incorrect password!");

                    var hashedPassword = HashPassword(model.new_password); //Hash password

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
