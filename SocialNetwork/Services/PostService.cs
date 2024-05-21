using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Models.DTO;
using SocialNetwork.Data;
using SocialNetwork.Models;

namespace SocialNetwork.Services
{
    public class PostService
    {
        private readonly UserDbContext _context;

        public PostService(UserDbContext context)
        {
            _context = context;
        }

        public PostDTO CreatePost(int authorId, string content, List<PostTag> tags)
        {
            var post = new Post
            {
                AuthorId = authorId,
                Content = content,
                DatePosted = DateTime.UtcNow,
                Tags = tags
            };

            _context.Posts.Add(post);
            _context.SaveChanges();

            return new PostDTO
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                Content = post.Content,
                DatePosted = post.DatePosted,
                LikesCount = post.LikesCount,
                Tags = post.Tags
            };
        }

        public bool DeletePost(int postId)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddComment(int postId, CommentDTO commentDto)
        {
            var post = _context.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == postId);
            if (post != null)
            {
                var comment = new Comment
                {
                    PostId = postId,
                    UserId = commentDto.UserId,
                    Content = commentDto.Content,
                    Timestamp = DateTime.UtcNow
                };

                post.Comments.Add(comment);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<PostDTO> GetPostsByTag(PostTag tag)
        {
            return _context.Posts
                .Include(p => p.Comments)
                .Where(p => p.Tags.Contains(tag))
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    AuthorId = p.AuthorId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    LikesCount = p.LikesCount,
                    Tags = p.Tags,
                    Comments = p.Comments.Select(c => new CommentDTO
                    {
                        Id = c.Id,
                        PostId = c.PostId,
                        UserId = c.UserId,
                        Content = c.Content,
                        Timestamp = c.Timestamp
                    }).ToList()
                }).ToList();
        }

        public List<PostDTO> GetAllPosts()
        {
            return _context.Posts
                .Include(p => p.Comments)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    AuthorId = p.AuthorId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    LikesCount = p.LikesCount,
                    Tags = p.Tags,
                    Comments = p.Comments.Select(c => new CommentDTO
                    {
                        Id = c.Id,
                        PostId = c.PostId,
                        UserId = c.UserId,
                        Content = c.Content,
                        Timestamp = c.Timestamp
                    }).ToList()
                }).ToList();
        }
    }
}