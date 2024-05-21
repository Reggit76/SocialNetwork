using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Models.DTO;
using SocialNetwork.Data;

namespace SocialNetwork.Services
{
    public class CommentService
    {
        private readonly UserDbContext _context;

        public CommentService(UserDbContext context)
        {
            _context = context;
        }

        public CommentDTO AddComment(int postId, int userId, string content, int? parentCommentId = null)
        {
            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = content,
                Timestamp = DateTime.UtcNow,
                ParentCommentId = parentCommentId
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return new CommentDTO
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserId = comment.UserId,
                Content = comment.Content,
                Timestamp = comment.Timestamp
            };
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

        public List<CommentDTO> GetCommentsForPost(int postId)
        {
            return _context.Comments
                .Where(c => c.PostId == postId)
                .Select(c => new CommentDTO
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    Content = c.Content,
                    Timestamp = c.Timestamp,
                    Replies = c.Replies.Select(r => new CommentDTO
                    {
                        Id = r.Id,
                        PostId = r.PostId,
                        UserId = r.UserId,
                        Content = r.Content,
                        Timestamp = r.Timestamp
                    }).ToList()
                }).ToList();
        }
    }
}