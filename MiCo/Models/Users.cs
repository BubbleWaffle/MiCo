using System.ComponentModel.DataAnnotations;

namespace MiCo.Models
{
    public class Users
    {
        [Key] public int id { get; set; }

        [Required]
        public string nickname { get; set; } = null!;

        [Required]
        public string login { get; set; } = null!;

        [Required]
        public string password { get; set; } = null!;

        [Required]
        public string email { get; set; } = null!;

        [Required]
        public DateTimeOffset creation_date { get; set; }

        public string? pfp { get; set; }

        [Required]
        public int role { get; set; }

        [Required]
        public int status { get; set; }

        public List<Threads>? user_threads { get; set; }
    }
}
