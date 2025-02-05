﻿using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Gender Gender { get; set; }
        public string? AvatarUrl { get; set; }

        public string FullName { get; set; } = null!;
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public Role Role { get; set; }
    }
}
