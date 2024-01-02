using MiCo.Models;
using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public interface IHomeService
    {
        Task<List<ThreadsAndScoreViewModel>> HomeThreads(string search, string sort_option);
        Task<List<Users>> HomeWOF(int no_3);
    }
}
