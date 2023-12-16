using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class RegistrationViewModel
    {
        [EmailAddress]
        public string? email { get; set; }

        [Required]
        public string? login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? confirm_password { get; set; }
    }
}
