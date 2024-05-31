using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.Entity;
using SocialNetwork.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Services
{
    public class PostService : IPostService
    {
        private readonly UserDbContext _context;

        public PostService(UserDbContext context)
        {
            _context = context;
        }

        public List<PostDTO> GetAllPosts()
        {
            var posts = _context.Posts
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .AsSplitQuery() // Вручную указываем разделение запросов
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    LikesCount = p.LikesCount,
                    Tags = p.Tags.Select(t => t.Tag).ToList(),
                    Comments = p.Comments.Select(c => new CommentDTO
                    {
                        Id = c.Id,
                        PostId = c.PostId,
                        UserId = c.UserId,
                        Content = c.Content,
                        DatePosted = c.DatePosted
                    }).ToList()
                }).ToList();

            return posts;
        }

        public PostDTO GetPostById(int postId)
        {
            var post = _context.Posts
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .AsSplitQuery() // Вручную указываем разделение запросов
                .FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                return null;
            }

            return new PostDTO
            {
                Id = post.Id,
                UserId = post.UserId,
                Content = post.Content,
                DatePosted = post.DatePosted,
                LikesCount = post.LikesCount,
                Tags = post.Tags.Select(t => t.Tag).ToList(),
                Comments = post.Comments.Select(c => new CommentDTO
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    Content = c.Content,
                    DatePosted = c.DatePosted
                }).ToList()
            };
        }

        public void CreatePost(PostDTO postDTO)
        {
            if (!_context.UserProfiles.Any(up => up.UserId == postDTO.UserId))
            {
                throw new ArgumentException("UserId does not exist in UserProfiles.");
            }

            var post = new Post
            {
                UserId = postDTO.UserId,
                Content = postDTO.Content,
                DatePosted = DateTime.UtcNow,
                LikesCount = 0
            };

            post.Tags = postDTO.Tags.Select(tag => new PostTag
            {
                Post = post,
                Tag = tag
            }).ToList();

            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public void UpdatePost(PostDTO postDTO)
        {
            var post = _context.Posts
                .Include(p => p.Tags)
                .FirstOrDefault(p => p.Id == postDTO.Id);

            if (post != null)
            {
                post.Content = postDTO.Content;

                post.Tags.Clear();
                post.Tags = postDTO.Tags.Select(tag => new PostTag
                {
                    PostId = post.Id,
                    Tag = tag
                }).ToList();

                _context.SaveChanges();
            }
        }

        public void DeletePost(int postId)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
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
    }
}