using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ProfileDeleteViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; } = null!;
    }
}
