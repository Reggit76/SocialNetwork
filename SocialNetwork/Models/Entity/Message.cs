using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.Entity
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int SenderId { get; set; }
        public int ReceverId { get; set; }
    }
}
