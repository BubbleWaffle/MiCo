namespace MiCo.Models.ViewModels
{
    public class ThreadViewModel
    {
        public int? id_liked_thread { get; set; }
        public int? id_disliked_thread { get; set; }
        public int LikeDislikeStatus { get; set; }
        public int _score { get; set; }
        public Threads _OGThread { get; set; } = null!;
        public List<ThreadViewModel> _replies { get; set; } = new List<ThreadViewModel>();
    }
}
