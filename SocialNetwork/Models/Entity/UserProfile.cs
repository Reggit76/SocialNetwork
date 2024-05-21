namespace SocialNetwork.Models.Entity
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; } 
        public UserRole Role { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<ChatUser> Chats { get; set; } = new List<ChatUser>();

        public UserProfile() { }
    }
}
