using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MiCo.Models
{
    public class Reports
    {
        [Key] public int id { get; set; }

        [ForeignKey("reported_user")]
        public int id_reported_user { get; set; }
        public Users reported_user { get; set; } = null!;

        [ForeignKey("reporting_user")]
        public int id_reporting_user { get; set; }
        public Users reporting_user { get; set; } = null!;

        [Required]
        public string reason { get; set; } = null!;

        [Required]
        public DateTimeOffset report_date { get; set; }
    }
}
