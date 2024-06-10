using SocialNetwork.Models.DTO;

namespace SocialNetwork.Models.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public Role Role { get; set; }
        public bool IsBanned { get; set; } 

        public virtual ICollection<ChatUser> Chats { get; set; } = new List<ChatUser>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();

        public User() { }
    }
}
