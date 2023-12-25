namespace MiCo.Models.ViewModels
{
    public class ProfileViewModel
    {
        public ProfileEditViewModel? ProfileEdit { get; set; }
        public ProfileContentViewModel ProfileContent { get; set; } = null!;
    }
}
