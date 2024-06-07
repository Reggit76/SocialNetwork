using System.Collections.Generic;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface IPostService
    {
        IEnumerable<PostDTO> GetAllPosts();
        PostDTO GetPostById(int id);
        void CreatePost(PostDTO postDTO);
        void UpdatePost(PostDTO postDTO);
        void DeletePost(int id);
        void AddComment(CommentDTO commentDTO);
        void UpdateComment(CommentDTO commentDTO);
        void DeleteComment(int commentId);
        IEnumerable<PostDTO> GetUserPosts(int userId);
    }
}
