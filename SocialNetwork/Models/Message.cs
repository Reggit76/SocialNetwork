using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class Message
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int SenderId { get; set; }
        [Required]
        public int ReceverId { get; set; }
    }
}
