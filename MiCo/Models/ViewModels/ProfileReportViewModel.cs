using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ProfileReportViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string reason { get; set; } = null!;
    }
}
