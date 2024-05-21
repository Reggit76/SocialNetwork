namespace SocialNetwork.Models.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
