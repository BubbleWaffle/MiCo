using MiCo.Helpers;
using MiCo.Models;
using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public interface IThreadService
    {
        Task<ResultHelper> ThreadCreate(int? id, ThreadCreateViewModel model);
        Task<ThreadViewModel> OGThreadContent(int id, int? userId);
        Task<List<ThreadViewModel>> RepliesContent(int id, int? userId);
        Task ThreadLike(int? id, int? user_id);
        Task ThreadDislike(int? id, int? user_id);
        Task<ResultHelper> ThreadReply(int OG_Thread_id, int? user_id, ThreadReplyViewModel model);
    }
}
