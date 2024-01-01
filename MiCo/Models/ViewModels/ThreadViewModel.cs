namespace MiCo.Models.ViewModels
{
    public class ThreadViewModel
    {
        public Threads _OGThread { get; set; } = null!;
        public List<ThreadViewModel> _replies { get; set; } = new List<ThreadViewModel>();
    }
}
