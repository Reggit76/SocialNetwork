using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.Entity;
using SocialNetwork.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<PostDTO> GetAllPosts()
        {
            return _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    LikesCount = p.LikesCount,
                    ImageUrl = p.ImageUrl // Учитываем ImageUrl
                })
                .ToList();
        }

        public PostDTO GetPostById(int id)
        {
            var post = _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == id);

            if (post == null) return null;

            return new PostDTO
            {
                Id = post.Id,
                UserId = post.UserId,
                Content = post.Content,
                DatePosted = post.DatePosted,
                LikesCount = post.LikesCount,
                ImageUrl = post.ImageUrl // Учитываем ImageUrl
            };
        }

        public void CreatePost(PostDTO postDTO)
        {
            var post = new Post
            {
                UserId = postDTO.UserId,
                Content = postDTO.Content,
                DatePosted = postDTO.DatePosted,
                LikesCount = postDTO.LikesCount,
                ImageUrl = postDTO.ImageUrl // Учитываем ImageUrl
            };

            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public void UpdatePost(PostDTO postDTO)
        {
            var post = _context.Posts.Find(postDTO.Id);
            if (post != null)
            {
                post.Content = postDTO.Content;
                post.DatePosted = postDTO.DatePosted;
                post.LikesCount = postDTO.LikesCount;
                post.ImageUrl = postDTO.ImageUrl; // Учитываем ImageUrl

                _context.SaveChanges();
            }
        }

        public void DeletePost(int id)
        {
            var post = _context.Posts.Find(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }
        }

        public void AddComment(CommentDTO commentDTO)
        {
            commentDTO.DatePosted = DateTime.UtcNow;
            var comment = new Comment
            {
                PostId = commentDTO.PostId,
                UserId = commentDTO.UserId,
                Content = commentDTO.Content,
                DatePosted = commentDTO.DatePosted
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

        public IEnumerable<PostDTO> GetUserPosts(int userId)
        {
            return _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.Author)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    LikesCount = p.LikesCount,
                    ImageUrl = p.ImageUrl 
                })
                .ToList();
        }
    }
}
