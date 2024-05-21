namespace SocialNetwork.Models.DTO
{
    public class PostDTO
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime DatePosted { get; set; }
        public int LikesCount { get; set; }
        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
        public List<PostTag> Tags { get; set; } = new List<PostTag>();
    }
}
