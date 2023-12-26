using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MiCo.Models.ViewModels
{
    public class ProfileEditViewModel
    {
        [DataType(DataType.Text)]
        public string? nickname { get; set; }

        [DataType(DataType.Text)]
        public string? login { get; set; }

        public IFormFile? file { get; set; }

        public string? delete_pfp { get; set; } //Checkbox value

        [Required]
        [DataType(DataType.Password)]
        public string? old_password { get; set; }

        [DataType(DataType.Password)]
        public string? new_password { get; set; }

        [DataType(DataType.Password)]
        public string? confirm_password { get; set; }
    }
}
