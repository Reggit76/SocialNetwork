using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface ICommentService
    {
        void AddComment(CommentDTO commentDTO);
        void DeleteComment(int commentId);
        CommentDTO GetCommentById(int commentId);
        void UpdateComment(CommentDTO commentDTO);
    }
}