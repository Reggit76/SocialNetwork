using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Models.ViewModels
{
    public class UserProfileViewModel
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
        public string? ProfilePictureUrl { get; set; }

        [Required]
        public Role Role { get; set; }

        public List<PostDTO> Posts { get; set; } = new List<PostDTO>();
    }
}
