namespace MiCo.Models.ViewModels
{
    public class ThreadViewModel
    {
        public Threads _OGThread { get; set; } = null!;
        public List<Threads> _replies { get; set; } = null!;
    }
}
