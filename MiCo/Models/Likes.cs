using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MiCo.Models
{
    public class Likes
    {
        [Key] public int id { get; set; }

        [ForeignKey("user")]
        public int id_user { get; set; }
        public virtual Users user { get; set; } = null!;

        [ForeignKey("thread")]
        public int id_thread { get; set; }
        public virtual Threads thread { get; set; } = null!;

        [Required]
        public int like_or_dislike { get; set; }
    }
}
