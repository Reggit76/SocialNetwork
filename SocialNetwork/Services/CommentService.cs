using SocialNetwork.Data;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.Entity;
using SocialNetwork.Services.Interfaces;
using System;
using System.Linq;

namespace SocialNetwork.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public CommentDTO GetCommentById(int commentId)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
            {
                return null;
            }
            return new CommentDTO
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserId = comment.UserId,
                Content = comment.Content,
                DatePosted = comment.DatePosted
            };
        }

        public void AddComment(CommentDTO commentDTO)
        {
            var comment = new Comment
            {
                PostId = commentDTO.PostId,
                UserId = commentDTO.UserId,
                Content = commentDTO.Content,
                DatePosted = DateTime.UtcNow // Преобразование в UTC
            };
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        public void UpdateComment(CommentDTO commentDTO)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == commentDTO.Id);
            if (comment != null)
            {
                comment.Content = commentDTO.Content;
                _context.SaveChanges();
            }
        }

        public void DeleteComment(int commentId)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
        }
    }
}