namespace SocialNetwork.Models.Entity
{
    public class ChatUser
    {
        public int ChatId { get; set; }
        public virtual Chat? Chat { get; set; }

        public int UserId { get; set; }
        public virtual UserProfile? UserProfile { get; set; }
    }
}
