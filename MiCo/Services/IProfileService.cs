using MiCo.Helpers;
using MiCo.Models;
using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public interface IProfileService
    {
        Task<ProfileContentViewModel> ProfileContent(string? login);
        Task<List<Threads>> ProfileThreads(string? login);
        Task<ResultHelper> ProfileEdit(int? id, ProfileEditViewModel model, bool delete_pfp);
        Task<ResultHelper> ProfileDelete(int? id, ProfileDeleteViewModel model);
        Task<ResultHelper> ProfileReport(int id_reported_user, int? id_reporting_user, ProfileReportViewModel model);
    }
}
