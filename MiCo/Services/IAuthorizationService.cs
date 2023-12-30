using MiCo.Helpers;
using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public interface IAuthorizationService
    {
        Task<ResultHelper> RegisterUser(RegistrationViewModel model);
        ResultHelper LoginUser(LoginViewModel model);
        bool LogoutUser();
    }
}
