using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ProfileEditViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string nickname { get; set; } = null!;

        [Required]
        [DataType(DataType.Text)]
        public string login { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string? old_password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? new_password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? confirm_password { get; set; }
    }
}
