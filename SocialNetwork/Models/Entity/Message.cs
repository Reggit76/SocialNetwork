using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.Entity
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Timestamp { get; set; }

        public Message(int senderId, int chatId, string content)
        {
            SenderId = senderId;
            ChatId = chatId;
            Content = content;
            Timestamp = DateTime.UtcNow;
        }
    }
}
