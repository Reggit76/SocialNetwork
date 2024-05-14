namespace SocialNetwork.Models.Entity
{
    public class Post
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public int LikesCount { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public List<PostTag> Tags { get; set; } // Список тегов

        // Конструктор, обновленный для включения списка тегов
        public Post(int authorId, string content, List<PostTag> tags)
        {
            AuthorId = authorId;
            Content = content;
            DatePosted = DateTime.Now;
            LikesCount = 0;
            Comments = new List<Comment>();
            Tags = tags ?? new List<PostTag>(); // Инициализация пустым списком, если теги не предоставлены
        }
    }
}
