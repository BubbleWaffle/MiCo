using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ProfileContentViewModel
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

        [Required]
        public int number_of_threads { get; set; }

        [Required]
        public int score { get; set; }
    }
}
