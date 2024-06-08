using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.Entity;
using SocialNetwork.Services.Interfaces;

namespace SocialNetwork.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostDTO>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    LikesCount = p.LikesCount,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<PostDTO> GetPostByIdAsync(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return null;

            return new PostDTO
            {
                Id = post.Id,
                UserId = post.UserId,
                Content = post.Content,
                DatePosted = post.DatePosted,
                LikesCount = post.LikesCount,
                ImageUrl = post.ImageUrl
            };
        }

        public async Task CreatePostAsync(PostDTO postDTO)
        {
            var post = new Post
            {
                UserId = postDTO.UserId,
                Content = postDTO.Content,
                DatePosted = postDTO.DatePosted,
                LikesCount = postDTO.LikesCount,
                ImageUrl = postDTO.ImageUrl
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(PostDTO postDTO)
        {
            var post = await _context.Posts.FindAsync(postDTO.Id);
            if (post != null)
            {
                post.Content = postDTO.Content;
                post.DatePosted = postDTO.DatePosted;
                post.LikesCount = postDTO.LikesCount;
                post.ImageUrl = postDTO.ImageUrl;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddCommentAsync(CommentDTO commentDTO)
        {
            commentDTO.DatePosted = DateTime.UtcNow;
            var comment = new Comment
            {
                PostId = commentDTO.PostId,
                UserId = commentDTO.UserId,
                Content = commentDTO.Content,
                DatePosted = commentDTO.DatePosted
            };
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(CommentDTO commentDTO)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentDTO.Id);
            if (comment != null)
            {
                comment.Content = commentDTO.Content;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PostDTO>> GetUserPostsAsync(int userId)
        {
            return await _context.Posts
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
                .ToListAsync();
        }
    }
}
