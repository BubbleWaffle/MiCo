using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ThreadEditViewModel
    {
        public int OG_thread = -1;

        [Required]
        public string title { get; set; } = null!;

        [Required]
        public string description { get; set; } = null!;

        public string? tags { get; set; }

        public List<IFormFile>? files { get; set; }

        public string? delete_files { get; set; } //Checkbox value
    }
}
