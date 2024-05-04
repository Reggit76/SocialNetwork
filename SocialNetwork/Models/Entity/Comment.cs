namespace SocialNetwork.Models.Entity
{
    public class Comment
    {
        public int Id { get; set; }
        public int? ParentCommentId { get; set; } // Ссылка на родительский комментарий
        public int AuthorId { get; set; }
        public int PostId { get; set; } // Идентификатор поста, к которому относится комментарий
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public List<Comment> Replies { get; set; } = new List<Comment>(); // Связанные ответы (вложенные комментарии)

        public void AddReply(Comment reply)
        {
            reply.ParentCommentId = this.Id;
            Replies.Add(reply);
        }
    }

}
