using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ProfileEditViewModel
    {
        [DataType(DataType.Text)]
        public string? nickname { get; set; }

        [DataType(DataType.Text)]
        public string? login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? old_password { get; set; }

        [DataType(DataType.Password)]
        public string? new_password { get; set; }

        [DataType(DataType.Password)]
        public string? confirm_password { get; set; }
    }
}
