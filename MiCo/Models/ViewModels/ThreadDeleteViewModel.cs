using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ThreadDeleteViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; } = null!;

        public int thread_id { get; set; }
    }
}
