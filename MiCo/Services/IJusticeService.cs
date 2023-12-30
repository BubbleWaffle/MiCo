using MiCo.Helpers;
using MiCo.Models;
using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public interface IJusticeService
    {
        Task<JusticeViewModel> JusticeContent();
        Task<ResultHelper> JusticeBan(Users banned_user, int? id_moderator, BanViewModel model);
        Task<ResultHelper> JusticeUnban(int id);
        void Unban();
        Task<ResultHelper> JusticeSave(int id);
    }
}
