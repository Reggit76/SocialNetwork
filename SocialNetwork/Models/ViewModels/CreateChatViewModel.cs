using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels
{
    public class CreateChatViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string Description { get; set; } = null!;
        public string? ChatIconUrl { get; set; }

        public List<int> ParticipantIds { get; set; } = new List<int>();
    }
}
