using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MiCo.Models
{
    public class Tokens
    {
        [Key] public int id { get; set; }

        [ForeignKey("user")]
        public int id_user { get; set; }
        public Users user { get; set; } = null!;

        [Required]
        public string token { get; set; } = null!;

        [Required]
        public DateTimeOffset expiration_date { get; set; }

        [Required]
        public bool expired { get; set; } = false;

        [Required]
        public int type { get; set; }
    }
}
