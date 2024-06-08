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

        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public Role Role { get; set; }
    }
}
