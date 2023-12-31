using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        public ProfileContentViewModel _profileContent { get; set; } = null!;
        public List<Threads> _profileThreads { get; set; } = null!;
    }
}
