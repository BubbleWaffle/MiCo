namespace MiCo.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<ThreadsAndScoreViewModel> _listOfThreads { get; set; } = null!;
        public List<Users> _topUsers { get; set; } = null!;
    }
}