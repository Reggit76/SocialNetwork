namespace SocialNetwork.Models.Entity
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime DatePosted { get; set; }
        public int LikesCount { get; set; }
        public string? ImageUrl { get; set; }

        public virtual User? Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public Post() { }
    }
}
