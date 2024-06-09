using SocialNetwork.Models.Entity;

namespace SocialNetwork.Models.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Description { get; set; }
        public Role Role { get; set; }
        public bool IsBanned { get; set; }

        public virtual ICollection<PostDTO> Posts { get; set; } = new List<PostDTO>();
        public virtual ICollection<FriendshipDTO> Friendships { get; set; } = new List<FriendshipDTO>();
    }
}
