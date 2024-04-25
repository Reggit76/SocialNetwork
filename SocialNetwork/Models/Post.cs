using System.Xml.Linq;

namespace SocialNetwork.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public string ImageVideoPath { get; set; }

        // Сложнее отношения
        public virtual User Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
