using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; } = null!;

        [Required]
        [DataType (DataType.Text)]
        public string login { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string confirm_password { get; set; } = null!;
    }
}
