namespace SocialNetwork.Models.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Description { get; set; }
        public Role Role { get; set; }
        public bool IsBanned { get; set; }
    }
}
