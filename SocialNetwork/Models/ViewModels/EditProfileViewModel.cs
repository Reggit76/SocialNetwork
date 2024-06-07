namespace SocialNetwork.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string Description { get; set; }
    }
}
