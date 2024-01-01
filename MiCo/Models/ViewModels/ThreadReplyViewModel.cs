using System.ComponentModel.DataAnnotations;

namespace MiCo.Models.ViewModels
{
    public class ThreadReplyViewModel
    {
        [Required]
        public string reply_description { get; set; } = null!;

        public List<IFormFile>? reply_files { get; set; }
    }
}
