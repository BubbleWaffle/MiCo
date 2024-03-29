﻿using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models.ViewModels;
using MiCo.Models;
using Microsoft.EntityFrameworkCore;

namespace MiCo.Services
{
    public class ProfileService : IProfileService
    {
        private readonly MiCoDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProfileService(MiCoDbContext context, IHttpContextAccessor contextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Method used to load content to specific profile
        /// </summary>
        /// <param name="login">User account name passing by URL</param>
        /// <returns>ProfileContentViewModel with data</returns>
        public async Task<ProfileContentViewModel> ProfileContent(string? login)
        {
            var user = _context.users.FirstOrDefault(u => u.login == login);

            if (user == null)
                return new ProfileContentViewModel();

            string pfp_url = user.pfp ?? "../content/default/pfp_default.svg";

            int threads = await _context.threads
                .CountAsync(t => t.author.login == login && !t.deleted);

            int fullScore = await _context.likes
                .GroupJoin(
                    _context.threads.Where(t => t.author.login == login && !t.deleted),
                    like => like.thread.id,
                    thread => thread.id,
                    (like, threads) => new { Like = like, Threads = threads }
                )
                .Where(joined => joined.Threads.Any())
                .SumAsync(joined => joined.Like.like_or_dislike);

            var profileContentViewModel = new ProfileContentViewModel
            {
                nickname = user.nickname,
                login = user.login,
                creation_date = user.creation_date.DateTime,
                pfp = pfp_url,
                role = user.role,
                number_of_threads = threads,
                score = fullScore
            };

            return profileContentViewModel;
        }

        /// <summary>
        /// Method used to load List of current profile threads
        /// </summary>
        /// <param name="login">User account name passing by URL</param>
        /// <returns>List of threads</returns>
        public async Task<List<ThreadsAndScoreViewModel>> ProfileThreads(string? login)
        {
            var threadsList = await _context.threads
                .Include(t => t.author)
                .Include(t => t.thread_tags!)
                    .ThenInclude(tt => tt.tag)
                .Include(t => t.thread_images)
                .Where(t => t.author.login == login && t.id_reply == null && t.id_OG_thread == null && !t.deleted && t.author.status != -1 && t.author.status != 1)
                .OrderBy(t => t.creation_date)
                .ToListAsync();

            var threadsWithScores = new List<ThreadsAndScoreViewModel>();

            foreach (var thread in threadsList)
            {
                var threadViewModel = new ThreadsAndScoreViewModel
                {
                    _thread = thread,
                    _score = await GetScore(thread.id)
                };

                threadsWithScores.Add(threadViewModel);
            }

            return threadsWithScores;
        }

        /// <summary>
        /// Method of calculating score
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <returns>Score</returns>
        private async Task<int> GetScore(int threadId)
        {
            var likesCount = await _context.likes.CountAsync(l => l.id_thread == threadId && l.like_or_dislike == 1);
            var dislikesCount = await _context.likes.CountAsync(l => l.id_thread == threadId && l.like_or_dislike == -1);

            return likesCount - dislikesCount;
        }

        /// <summary>
        /// Method used to edit user data
        /// </summary>
        /// <param name="id">Id currently logged user</param>
        /// <param name="model">View model passing edit data</param>
        /// <param name="delete_pfp">True if delete avatar else leave or change</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> ProfileEdit(int? id, ProfileEditViewModel model, bool delete_pfp)
        {
            var user = _context.users.FirstOrDefault(u => u.id == id);

            if (user != null)
            {
                if (string.IsNullOrWhiteSpace(model.old_password))
                    return new ResultHelper(false, "Please enter your current password!");

                if (!Utilities.Tools.VerifyPassword(model.old_password, user.password))
                    return new ResultHelper(false, "Incorrect password!");

                if (!string.IsNullOrWhiteSpace(model.nickname)) user.nickname = model.nickname;


                if (!string.IsNullOrWhiteSpace(model.login))
                {
                    if (model.login.Length < 4 || model.login.Length > 14)
                        return new ResultHelper(false, "Login is too short or too long (min 4 characters, max 14 characters)!");

                    if (!Utilities.Tools.IsValidLoginOrNickname(model.login))
                        return new ResultHelper(false, "Invalid login!");

                    if (_context.users.Any(u => u.login == model.login))
                        return new ResultHelper(false, "Provided login is already in use or is the same as current!");

                    user.login = model.login;
                }

                if (model.file != null && model.file.Length > 0)
                {
                    if (model.file.Length > 15728640)
                        return new ResultHelper(false, "Image is too big (MAX 15MB)!");

                    string baseFileName = $"{id}";

                    string uniqueFileName = $"{id}.{Path.GetExtension(model.file.FileName)}";

                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "content", "pfp");

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    foreach (var fileToDelete in Directory.GetFiles(uploadsFolder, $"{baseFileName}.*"))
                    {
                        File.Delete(fileToDelete);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.file.CopyToAsync(fileStream);
                    }

                    user.pfp = $"../content/pfp/{uniqueFileName}";
                }

                if (delete_pfp) user.pfp = null;

                if (!string.IsNullOrWhiteSpace(model.new_password))
                {
                    if (string.IsNullOrWhiteSpace(model.confirm_password))
                        return new ResultHelper(false, "You have to confirm your new password!");

                    if (!Utilities.Tools.IsValidPassword(model.new_password))
                        return new ResultHelper(false, "Password must contain special characters and numbers!");

                    if (model.new_password.Length < 8)
                        return new ResultHelper(false, "Password is too short (min 8 characters)!");

                    if (model.confirm_password != model.new_password)
                        return new ResultHelper(false, "New password does not match!");

                    if (model.old_password == model.new_password)
                        return new ResultHelper(false, "New password cannot be the same as the old one!");

                    var hashedPassword = Utilities.Tools.HashPassword(model.new_password);

                    user.password = hashedPassword;
                }

                _context.users.Update(user);
                await _context.SaveChangesAsync();

                UpdateSession(user);

                return new ResultHelper(true, "Changes saved!");
            }

            return new ResultHelper(false, "Something went wrong!");
        }

        /// <summary>
        /// Method used to update current logged user session
        /// </summary>
        /// <param name="user">Users object with new data</param>
        private void UpdateSession(Users user)
        {
            _contextAccessor.HttpContext?.Session.SetString("Nickname", user.nickname);
            _contextAccessor.HttpContext?.Session.SetString("Login", user.login);
            _contextAccessor.HttpContext?.Session.SetInt32("Role", user.role);
            if (user.pfp != null) _contextAccessor.HttpContext?.Session.SetString("PFP", user.pfp);
            else _contextAccessor.HttpContext?.Session.SetString("PFP", "../content/default/pfp_default.svg");
        }

        /// <summary>
        /// Method used to delete profile
        /// </summary>
        /// <param name="id">Id currently logged user</param>
        /// <param name="model">View model passing delete data</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> ProfileDelete(int? id, ProfileDeleteViewModel model)
        {
            var user = _context.users.FirstOrDefault(u => u.id == id);

            if (user != null && Utilities.Tools.VerifyPassword(model.password, user.password) && !string.IsNullOrWhiteSpace(model.password))
            {
                user.status = -1;

                var threadsToSoftDelete = _context.threads
                    .Where(t => t.id_author == user.id && !t.deleted);

                foreach (var thread in threadsToSoftDelete)
                {
                    thread.deleted = true;
                    _context.threads.Update(thread);
                }

                var likesToDelete = _context.likes
                    .Where(l => l.id_user == user.id);

                _context.likes.RemoveRange(likesToDelete);

                _context.users.Update(user);
                await _context.SaveChangesAsync();

                _contextAccessor.HttpContext?.Session.Clear();

                return new ResultHelper(true, "Your account has been deleted.");
            }

            return new ResultHelper(false, "Incorrect password!");
        }

        /// <summary>
        /// Method used to report specific user
        /// </summary>
        /// <param name="id_reported_user">Id reported user</param>
        /// <param name="id_reporting_user">Id reporting user (currently logged user)</param>
        /// <param name="model">View model passing report data</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> ProfileReport(int id_reported_user, int? id_reporting_user, ProfileReportViewModel model)
        {
            int non_nullable_id = id_reporting_user ?? default(int); //Convert nullable int value to non-nullable

            var existing_report = _context.reports
                .Where(r => r.id_reporting_user == non_nullable_id && r.id_reported_user == id_reported_user &&
                r.report_date >= DateTimeOffset.Now.AddDays(-1))
                .FirstOrDefault();

            if (existing_report != null)
                return new ResultHelper(false, "You have recently reported this user! Take a break.");

            if (string.IsNullOrWhiteSpace(model.reason))
                return new ResultHelper(false, "You have to enter reason!");

            if (model.reason.Length > 300)
                return new ResultHelper(false, "Your reason is too long!");

            var newReport = new Reports
            {
                id_reported_user = id_reported_user,
                id_reporting_user = non_nullable_id,
                reason = model.reason,
                report_date = DateTimeOffset.Now
            };

            _context.reports.Add(newReport);
            await _context.SaveChangesAsync();

            return new ResultHelper(true, "We received your report!");
        }
    }
}
