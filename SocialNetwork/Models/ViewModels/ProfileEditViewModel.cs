using System;
using System.ComponentModel.DataAnnotations;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Models.ViewModels
{
    public class ProfileEditViewModel
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters.")]
        public string FullName { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Url]
        [StringLength(255, ErrorMessage = "Profile picture URL cannot be longer than 255 characters.")]
        public string ProfilePictureUrl { get; set; }
    }
}
