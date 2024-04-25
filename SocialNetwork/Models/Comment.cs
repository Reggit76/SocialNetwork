namespace SocialNetwork.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }

        // Сложнее отношения
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
