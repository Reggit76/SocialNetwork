using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels
{
    public class EditChatViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        public string Description { get; set; }

        [Url]
        [Display(Name = "Chat Icon URL")]
        public string? ChatIconUrl { get; set; }

        public List<int> ParticipantIds { get; set; }
    }
}
