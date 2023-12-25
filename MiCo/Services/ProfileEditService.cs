﻿using MiCo.Data;
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

        public ProfileEditService(MiCoDbContext context, IHttpContextAccessor contextAccessor)
        {  
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<ResultHelper> EditProfile(int? id, string? nickname, string? login, string? old_password, string? new_password, string? confirm_password)
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

                if (!string.IsNullOrWhiteSpace(new_password) && !string.IsNullOrWhiteSpace(confirm_password))
                {
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
