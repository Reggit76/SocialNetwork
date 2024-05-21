namespace SocialNetwork.Models.DTO
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public List<CommentDTO> Replies { get; set; } = new List<CommentDTO>();
    }
}
