using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
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

        public Post CreatePost(int authorId, string content, List<PostTag> tags)
        {
            var post = new Post(authorId, content, tags);
            _context.Posts.Add(post);
            _context.SaveChanges();

            return post;
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

        public bool AddComment(int postId, Comment comment)
        {
            var post = _context.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == postId);
            if (post != null)
            {
                post.Comments.Add(comment);
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public List<Post> GetPostsByTag(PostTag tag)
        {
            return _context.Posts.Include(p => p.Comments).Where(p => p.Tags.Contains(tag)).ToList();
        }

        public List<Post> GetAllPosts()
        {
            return _context.Posts.Include(p => p.Comments).ToList();
        }
    }
}