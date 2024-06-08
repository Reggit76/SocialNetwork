using System;
using System.ComponentModel.DataAnnotations;
using SocialNetwork.Models;

namespace SocialNetwork.Models.ViewModels
{
    public class RegisterAdminViewModel
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Url]
        public string ProfilePictureUrl { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}
