using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocialNetwork.Data;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.Entity;
using SocialNetwork.Services.Interfaces;

namespace SocialNetwork.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5); // Время жизни кэша

        public PostService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<PostDTO>> GetAllPostsAsync()
        {
            return await _cache.GetOrCreateAsync("all_posts", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
                return _context.Posts
                    .OrderByDescending(p => p.DatePosted)
                    .Include(p => p.Author)
                    .Include(p => p.Comments)
                    .Select(p => new PostDTO
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        Content = p.Content,
                        DatePosted = p.DatePosted,
                        LikesCount = p.LikesCount,
                        ImageUrl = p.ImageUrl,
                        AuthorProfile = new UserDTO
                        {
                            Id = p.Author.Id,
                            FullName = p.Author.FullName,
                            ProfilePictureUrl = p.Author.ProfilePictureUrl
                        },
                        Comments = p.Comments.Select(c => new CommentDTO
                        {
                            Id = c.Id,
                            PostId = c.PostId,
                            UserId = c.UserId,
                            Content = c.Content,
                            DatePosted = c.DatePosted,
                            UserFullName = c.User.FullName,
                            UserProfilePictureUrl = c.User.ProfilePictureUrl
                        }).ToList()
                    })
                    .ToListAsync();
            });
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
                ImageUrl = post.ImageUrl,
                AuthorProfile = new UserDTO
                {
                    Id = post.Author.Id,
                    FullName = post.Author.FullName,
                    ProfilePictureUrl = post.Author.ProfilePictureUrl
                },
                Comments = post.Comments.Select(c => new CommentDTO
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    Content = c.Content,
                    DatePosted = c.DatePosted,
                    UserFullName = c.User.FullName,
                    UserProfilePictureUrl = c.User.ProfilePictureUrl
                }).ToList()
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
            _cache.Remove("all_posts"); // 
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
                _cache.Remove("all_posts"); 
            }
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                _cache.Remove("all_posts"); 
            }
        }

        public async Task<bool> LikePostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                post.LikesCount++;
                await _context.SaveChangesAsync();
                _cache.Remove("all_posts"); 
                return true;
            }
            return false;
        }

        public async Task<bool> DislikePostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                post.LikesCount--;
                await _context.SaveChangesAsync();
                _cache.Remove("all_posts"); 
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<PostDTO>> GetUserPostsAsync(int userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DatePosted) 
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    LikesCount = p.LikesCount,
                    ImageUrl = p.ImageUrl,
                    AuthorProfile = new UserDTO
                    {
                        Id = p.Author.Id,
                        FullName = p.Author.FullName,
                        ProfilePictureUrl = p.Author.ProfilePictureUrl
                    },
                    Comments = p.Comments.Select(c => new CommentDTO
                    {
                        Id = c.Id,
                        PostId = c.PostId,
                        UserId = c.UserId,
                        Content = c.Content,
                        DatePosted = c.DatePosted,
                        UserFullName = c.User.FullName,
                        UserProfilePictureUrl = c.User.ProfilePictureUrl
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PostDTO>> GetFriendsPostsAsync(int userId)
        {
            var friendIds = await _context.Friendships
                .Where(f => f.UserId == userId)
                .Select(f => f.FriendId)
                .ToListAsync();

            var cacheKey = $"friends_posts_{userId}";
            return await _cache.GetOrCreateAsync(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
                return _context.Posts
                    .Where(p => friendIds.Contains(p.UserId))
                    .OrderByDescending(p => p.DatePosted) // 
                    .Include(p => p.Author)
                    .Include(p => p.Comments)
                    .Select(p => new PostDTO
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        Content = p.Content,
                        DatePosted = p.DatePosted,
                        LikesCount = p.LikesCount,
                        ImageUrl = p.ImageUrl,
                        AuthorProfile = new UserDTO
                        {
                            Id = p.Author.Id,
                            FullName = p.Author.FullName,
                            ProfilePictureUrl = p.Author.ProfilePictureUrl
                        },
                        Comments = p.Comments.Select(c => new CommentDTO
                        {
                            Id = c.Id,
                            PostId = c.PostId,
                            UserId = c.UserId,
                            Content = c.Content,
                            DatePosted = c.DatePosted,
                            UserFullName = c.User.FullName,
                            UserProfilePictureUrl = c.User.ProfilePictureUrl
                        }).ToList()
                    })
                    .ToListAsync();
            });
        }
    }
}
