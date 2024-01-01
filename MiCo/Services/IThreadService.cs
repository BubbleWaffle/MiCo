using MiCo.Helpers;
using MiCo.Models;
using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public interface IThreadService
    {
        Task<ResultHelper> ThreadCreate(int? id, ThreadCreateViewModel model);
        Task<Threads> OGThreadContent(int id);
        Task<List<Threads>> RepliesContent(int id);
    }
}
