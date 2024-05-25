namespace SocialNetwork.Models.Entity
{
    public class Post
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime DatePosted { get; set; }
        public int LikesCount { get; set; }

        public virtual UserProfile? Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<PostTag> Tags { get; set; } = new List<PostTag>();

        public Post() { }
    }
}
