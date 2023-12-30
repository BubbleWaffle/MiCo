using MiCo.Helpers;
using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public interface IThreadService
    {
        Task<ResultHelper> ThreadCreate(int? id, ThreadCreateViewModel model);
    }
}
