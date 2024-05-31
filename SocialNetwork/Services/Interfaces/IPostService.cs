using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface IPostService
    {
        void AddComment(CommentDTO commentDTO);
        void CreatePost(PostDTO postDTO);
        void DeleteComment(int commentId);
        void DeletePost(int postId);
        List<PostDTO> GetAllPosts();
        PostDTO GetPostById(int postId);
        void UpdateComment(CommentDTO commentDTO);
        void UpdatePost(PostDTO postDTO);
    }
}