﻿namespace SocialNetwork.Models.DTO
{
    public class PostDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime DatePosted { get; set; }
        public int LikesCount { get; set; }
        public string? ImageUrl { get; set; }
        public UserDTO? AuthorProfile { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
    }
}
