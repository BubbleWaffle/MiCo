using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        public string nickname { get; set; } = null!;

        [Required]
        public string login { get; set; } = null!;

        [Required]
        public DateTime creation_date { get; set; }

        [Required]
        public string pfp { get; set; } = null!;

        [Required]
        public int role { get; set; }
    }
}
