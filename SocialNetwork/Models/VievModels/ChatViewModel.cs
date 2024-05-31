using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels
{
    public class ChatViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Participants { get; set; } 
    }
}
