using System;
using System.ComponentModel.DataAnnotations;
using SocialNetwork.Models.Entity;

namespace SocialNetwork.Models.AccountViewModels
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Profile Picture URL")]
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}
