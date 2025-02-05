﻿namespace SocialNetwork.Models.Entity
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime DatePosted { get; set; }

        public virtual Post? Post { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

        public int? ParentCommentId { get; set; }
        public virtual Comment? ParentComment { get; set; }

        public Comment() { }
    }
}
