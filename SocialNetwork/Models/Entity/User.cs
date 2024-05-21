﻿namespace SocialNetwork.Models.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual UserProfile? UserProfile { get; set; }

        public User() { }
    }
}
