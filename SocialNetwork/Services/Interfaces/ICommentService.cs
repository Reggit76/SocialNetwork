using System.Threading.Tasks;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(CommentDTO commentDTO);
        Task DeleteCommentAsync(int commentId);
        Task<CommentDTO> GetCommentByIdAsync(int commentId);
        Task UpdateCommentAsync(CommentDTO commentDTO);
    }
}
