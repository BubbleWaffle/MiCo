﻿using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models.ViewModels;
using MiCo.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MiCo.Services
{
    public class ThreadService : IThreadService
    {
        private readonly MiCoDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ThreadService(MiCoDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Method used to create thread
        /// </summary>
        /// <param name="id">Id currently logged user</param>
        /// <param name="model">View model passing thread data</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> ThreadCreate(int? id, ThreadCreateViewModel model)
        {
            if (model != null && id != null)
            {
                string[]? tagsArray = null;

                if (string.IsNullOrWhiteSpace(model.title) && !Utilities.Tools.IsValidTitle(model.title))
                    return new ResultHelper(false, "Invalid title!");

                if (model.title.Length >= 75)
                    return new ResultHelper(false, "Title is too long (MAX 75 characters)!");

                if (string.IsNullOrWhiteSpace(model.description))
                    return new ResultHelper(false, "Description can't be empty!");

                if (model.description.Length > 500)
                    return new ResultHelper(false, "Description is too long (MAX 500 characters)!");

                if (model.files != null && model.files.Count > 3)
                    return new ResultHelper(false, "You can send only 3 files!");

                if (!string.IsNullOrWhiteSpace(model.tags))
                {
                    tagsArray = model.tags.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (!tagsArray.All(tag => Utilities.Tools.IsValidTags(tag)))
                        return new ResultHelper(false, "Invalid tags!");

                    tagsArray = tagsArray.Select(tag => $"#{tag.Trim()}").Distinct().ToArray();

                    if (tagsArray.Length > 9)
                        return new ResultHelper(false, "Too many tags (MAX 9)!");
                }

                var thread = new Threads
                {
                    id_author = id.Value,
                    title = model.title,
                    description = model.description,
                    creation_date = DateTimeOffset.Now,
                    deleted = false
                };

                _context.threads.Add(thread);
                await _context.SaveChangesAsync();

                if (tagsArray != null)
                {
                    foreach (var tagText in tagsArray)
                    {
                        var existingTag = await _context.tags.FirstOrDefaultAsync(t => t.tag == tagText);

                        if (existingTag == null)
                        {
                            var newTag = new Tags
                            {
                                tag = tagText
                            };
                            _context.tags.Add(newTag);
                            await _context.SaveChangesAsync();

                            var threadTag = new ThreadTags
                            {
                                id_thread = thread.id,
                                id_tag = newTag.id
                            };

                            _context.thread_tags.Add(threadTag);
                        }
                        else
                        {
                            var threadTag = new ThreadTags
                            {
                                id_thread = thread.id,
                                id_tag = existingTag.id
                            };

                            _context.thread_tags.Add(threadTag);
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                if (model.files != null && model.files.Count > 0)
                {
                    string threadFolderName = thread.id.ToString();
                    string threadFolder = Path.Combine(_hostEnvironment.WebRootPath, "content", "thread", threadFolderName);

                    if (!Directory.Exists(threadFolder))
                        Directory.CreateDirectory(threadFolder);

                    int counter = 1;

                    foreach (var file in model.files)
                    {
                        string uniqueFileName = $"{counter}{Path.GetExtension(file.FileName)}";
                        string filePath = Path.Combine(threadFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        var image = new Images
                        {
                            id_which_thread = thread.id,
                            image = $"../content/thread/{threadFolderName}/{uniqueFileName}"
                        };

                        _context.images.Add(image);
                        await _context.SaveChangesAsync();

                        counter++;
                    }
                }

                return new ResultHelper(true, "Thread created successfully!", thread.id);
            }

            return new ResultHelper(false, "You can't do that!");
        }

        public async Task<ThreadViewModel> OGThreadContent(int id, int? userId)
        {
            var og_thread = await _context.threads
                .Include(t => t.author)
                .Include(t => t.thread_images)
                .FirstOrDefaultAsync(t => t.id == id);

            var result = new ThreadViewModel
            {
                _OGThread = og_thread!,
                LikeDislikeStatus = userId.HasValue ? await GetUserLikeDislikeStatus(id, userId.Value) : 0
            };

            return result;
        }

        public async Task<List<ThreadViewModel>> RepliesContent(int id, int? userId)
        {
            var replies = await _context.threads
                .Include(t => t.author)
                .Include(t => t.thread_images)
                .Where(t => t.id_reply == id)
                .OrderBy(t => t.creation_date)
                .ToListAsync();

            var result = new List<ThreadViewModel>();

            foreach (var reply in replies)
            {
                var replyViewModel = new ThreadViewModel
                {
                    _OGThread = reply,
                    _replies = await RepliesContent(reply.id, userId),
                    LikeDislikeStatus = userId.HasValue ? await GetUserLikeDislikeStatus(reply.id, userId.Value) : 0
                };

                result.Add(replyViewModel);
            }

            return result;
        }

        public async Task<int> GetUserLikeDislikeStatus(int threadId, int userId)
        {
            var like = await _context.likes
                .Where(l => l.id_thread == threadId && l.id_user == userId)
                .Select(l => l.like_or_dislike)
                .FirstOrDefaultAsync();

            return like;
        }

        /// <summary>
        /// Method used to like or cancel like
        /// </summary>
        /// <param name="id">Thread id</param>
        /// <param name="user_id">User id</param>
        /// <returns>Do task</returns>
        public async Task ThreadLike(int? id, int? user_id)
        {
            int no_nullable_user_id = user_id ?? default(int);
            if (id.HasValue)
            {
                var existingLike = _context.likes.FirstOrDefault(l => l.id_thread == id && l.id_user == no_nullable_user_id);

                if (existingLike != null)
                {
                    existingLike.like_or_dislike = existingLike.like_or_dislike == 1 ? 0 : 1; // If like change to 0; if dislike change to 1
                    _context.Update(existingLike);
                }
                else
                {
                    var newLike = new Likes
                    {
                        id_user = no_nullable_user_id,
                        id_thread = id.Value,
                        like_or_dislike = 1
                    };

                    _context.Add(newLike);
                }

                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Method used to dislike or cancel dislike
        /// </summary>
        /// <param name="id">Thread id</param>
        /// <param name="user_id">User id</param>
        /// <returns>Do task</returns>
        public async Task ThreadDislike(int? id, int? user_id)
        {
            int no_nullable_user_id = user_id ?? default(int);
            if (id.HasValue)
            {
                var existingLike = _context.likes.FirstOrDefault(l => l.id_thread == id && l.id_user == no_nullable_user_id);

                if (existingLike != null)
                {
                    existingLike.like_or_dislike = existingLike.like_or_dislike == -1 ? 0 : -1; // If dislike change to 0; if like change to -1
                    _context.Update(existingLike);
                }
                else
                {
                    var newLike = new Likes
                    {
                        id_user = no_nullable_user_id,
                        id_thread = id.Value,
                        like_or_dislike = -1
                    };

                    _context.Add(newLike);
                }

                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Method used to reply
        /// </summary>
        /// <param name="reply_id">Thread id to replying</param>
        /// <param name="user_id">Replying user id</param>
        /// <param name="model">View model passing registration data</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> ThreadReply(int reply_id, int? user_id, ThreadReplyViewModel model)
        {
            var reply_thread = _context.threads.FirstOrDefault(o => o.id == reply_id);

            if (model != null && user_id != null && reply_thread != null)
            {
                if (string.IsNullOrWhiteSpace(model.reply_description))
                    return new ResultHelper(false, "Description can't be empty!");

                if (model.reply_description.Length > 500)
                    return new ResultHelper(false, "Description is too long (MAX 500 characters)!");

                if (model.reply_files != null && model.reply_files.Count > 3)
                    return new ResultHelper(false, "You can send only 3 files!");

                var thread = new Threads
                {
                    id_author = user_id.Value,
                    id_reply = reply_thread.id,
                    id_OG_thread = reply_thread.id_OG_thread ?? reply_thread.id,
                    title = reply_thread.title,
                    description = model.reply_description,
                    creation_date = DateTimeOffset.Now,
                    deleted = false
                };

                _context.threads.Add(thread);
                await _context.SaveChangesAsync();

                if (model.reply_files != null && model.reply_files.Count > 0)
                {
                    string threadFolderName = thread.id.ToString();
                    string threadFolder = Path.Combine(_hostEnvironment.WebRootPath, "content", "thread", threadFolderName);

                    if (!Directory.Exists(threadFolder))
                        Directory.CreateDirectory(threadFolder);

                    int counter = 1;

                    foreach (var file in model.reply_files)
                    {
                        string uniqueFileName = $"{counter}{Path.GetExtension(file.FileName)}";
                        string filePath = Path.Combine(threadFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        var image = new Images
                        {
                            id_which_thread = thread.id,
                            image = $"../content/thread/{threadFolderName}/{uniqueFileName}"
                        };

                        _context.images.Add(image);
                        await _context.SaveChangesAsync();

                        counter++;
                    }
                }

                int no_nullable = thread.id_OG_thread ?? default(int);

                return new ResultHelper(true, "Replied successfully!", no_nullable);
            }

            return new ResultHelper(false, "You can't do that!");
        }
    }
}
