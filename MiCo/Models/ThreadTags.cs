using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace MiCo.Models
{
    public class ThreadTags
    {
        [ForeignKey("thread")]
        public int id_thread { get; set; }
        public virtual Threads thread { get; set; } = null!;

        [ForeignKey("tag")]
        public int id_tag { get; set; }
        public virtual Tags tag { get; set; } = null!;
    }
}
