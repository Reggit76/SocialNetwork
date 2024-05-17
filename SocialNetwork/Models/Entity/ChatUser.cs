namespace SocialNetwork.Models.Entity
{
    public class ChatUser
    {
        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public int UserId { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
