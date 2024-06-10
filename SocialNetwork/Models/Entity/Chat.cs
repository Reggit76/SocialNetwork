namespace SocialNetwork.Models.Entity
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ChatIconUrl { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<ChatUser> Participants { get; set; } = new List<ChatUser>();

        public Chat() { }
    }
}
