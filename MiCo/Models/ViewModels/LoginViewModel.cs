using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string? email_or_login {  get; set; }

        [Required]
        [DataType (DataType.Password)]
        public string? password { get; set; }
    }
}
