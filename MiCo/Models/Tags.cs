using System.ComponentModel.DataAnnotations;

namespace MiCo.Models
{
    public class Tags
    {
        [Key] public int id { get; set; }

        [Required]
        public string tag { get; set; } = null!;

        public List<ThreadTags> thread_tags { get; set; } = null!;
    }
}
