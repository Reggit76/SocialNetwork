using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Models.Entity;
using SocialNetwork.Data;

public class CommentService
{
    private readonly UserDbContext _context;

    public CommentService(UserDbContext context)
    {
        _context = context;
    }

    public Comment AddComment(int postId, int userId, string content)
    {
        var comment = new Comment
        {
            PostId = postId,
            AuthorId = userId,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        _context.SaveChanges();
        return comment;
    }

    public bool DeleteComment(int commentId)
    {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return true;
        }
        return false;
    }

    public List<Comment> GetCommentsForPost(int postId)
    {
        return _context.Comments.Where(c => c.PostId == postId).ToList();
    }
}
