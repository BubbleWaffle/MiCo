using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public interface IHomeService
    {
        Task<HomeViewModel> HomeContent(string search, string sort_option, HomeViewModel model);
    }
}
