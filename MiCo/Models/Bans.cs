using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MiCo.Models
{
    public class Bans
    {
        [Key] public int id { get; set; }

        [ForeignKey("banned_user")]
        public int id_banned_user { get; set; }
        public Users banned_user { get; set; } = null!;

        [ForeignKey("moderator")]
        public int id_moderator { get; set; }
        public Users moderator { get; set; } = null!;

        [Required]
        public string reason { get; set; } = null!;

        [Required]
        public DateTimeOffset ban_date { get; set; }

        public DateTimeOffset? ban_until { get; set; }
    }
}
