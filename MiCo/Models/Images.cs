using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiCo.Models
{
    public class Images
    {
        [Key] public int id { get; set; }

        [ForeignKey("which_thread")]
        public int id_which_thread { get; set; }
        public virtual Threads which_thread { get; set; } = null!;

        [Required]
        public string image { get; set; } = null!;
    }
}
