using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.Entity
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
