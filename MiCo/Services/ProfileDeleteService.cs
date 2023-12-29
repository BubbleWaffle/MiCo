using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models.ViewModels;
using System.Security.Cryptography;

namespace MiCo.Services
{
    public class ProfileDeleteService
    {
        private readonly MiCoDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProfileDeleteService(MiCoDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        /* Delete logged user */
        public async Task<ResultHelper> ProfileDelete(int? id, ProfileDeleteViewModel model)
        {
            var user = _context.users.FirstOrDefault(u => u.id == id);

            if (user != null && VerifyPassword(model.password, user.password) && !string.IsNullOrWhiteSpace(model.password))
            {
                user.status = -1;

                _context.users.Update(user);
                await _context.SaveChangesAsync();

                _contextAccessor.HttpContext?.Session.Clear();

                return new ResultHelper(true, "Your account has been deleted.");
            }

            return new ResultHelper(false, "Incorrect password!");
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
