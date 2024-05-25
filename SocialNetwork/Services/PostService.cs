using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

public class PostService
{
    private readonly UserDbContext _context;
    private readonly CommentService _commentService;

    public PostService(UserDbContext context, CommentService commentService)
    {
        _context = context;
        _commentService = commentService;
    }

    public List<PostDTO> GetAllPosts()
    {
        return _context.Posts
            .Include(p => p.Comments)
            .Include(p => p.Tags)
            .Select(p => new PostDTO
            {
                Id = p.Id,
                AuthorId = p.AuthorId,
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
                    Timestamp = c.Timestamp
                }).ToList()
            }).ToList();
    }

    public PostDTO GetPostById(int postId)
    {
        var post = _context.Posts
            .Include(p => p.Comments)
            .Include(p => p.Tags)
            .FirstOrDefault(p => p.Id == postId);

        if (post == null)
        {
            return null;
        }

        return new PostDTO
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
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
                Timestamp = c.Timestamp
            }).ToList()
        };
    }

    public void CreatePost(PostDTO postDTO)
    {
        var post = new Post
        {
            AuthorId = postDTO.AuthorId,
            Content = postDTO.Content,
            DatePosted = DateTime.UtcNow, // Преобразование в UTC
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
        commentDTO.Timestamp = DateTime.UtcNow; // Преобразование в UTC
        _commentService.AddComment(commentDTO);
    }

    public void UpdateComment(CommentDTO commentDTO)
    {
        _commentService.UpdateComment(commentDTO);
    }

    public void DeleteComment(int commentId)
    {
        _commentService.DeleteComment(commentId);
    }
}
