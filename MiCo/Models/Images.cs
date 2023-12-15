using System.ComponentModel.DataAnnotations;

namespace MiCo.Models
{
    public class Images
    {
        [Key] public int id { get; set; }

        [Required]
        public string image { get; set; } = null!;

        public List<ThreadImages> thread_images { get; set; } = null!;
    }
}
