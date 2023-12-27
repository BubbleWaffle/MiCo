using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class BanViewModel
    {
        public DateTimeOffset? ban_until { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string reason { get; set; } = null!;
    }
}