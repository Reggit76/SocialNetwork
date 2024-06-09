using SocialNetwork.Models.DTO;
using SocialNetwork.Models;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Url]
        public string? ProfilePictureUrl { get; set; }

        [Required]
        public Role Role { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;


        public List<PostDTO> Posts { get; set; } = new List<PostDTO>();
    }
}