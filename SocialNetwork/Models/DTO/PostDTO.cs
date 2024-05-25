using System;
using System.Collections.Generic;

namespace SocialNetwork.Models.DTO
{
    public class PostDTO
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime DatePosted { get; set; }
        public int LikesCount { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
    }
}
