using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiCo.Models
{
    public class ThreadImages
    {
        [ForeignKey("thread")]
        public int id_thread { get; set; }
        public Threads thread { get; set; } = null!;

        [ForeignKey("image")]
        public int id_image { get; set; }
        public Images image { get; set; } = null!;
    }
}
