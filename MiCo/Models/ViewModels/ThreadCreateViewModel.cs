using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ThreadCreateViewModel
    {
        [Required]
        public string title { get; set; } = null!;

        [Required]
        public string description { get; set; } = null!;

        public List<IFormFile>? files { get; set; }
    }
}
