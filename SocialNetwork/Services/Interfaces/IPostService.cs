using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostDTO>> GetAllPostsAsync();
        Task<PostDTO> GetPostByIdAsync(int id);
        Task CreatePostAsync(PostDTO postDTO);
        Task UpdatePostAsync(PostDTO postDTO);
        Task DeletePostAsync(int id);
        Task AddCommentAsync(CommentDTO commentDTO);
        Task UpdateCommentAsync(CommentDTO commentDTO);
        Task DeleteCommentAsync(int commentId);
        Task<IEnumerable<PostDTO>> GetUserPostsAsync(int userId);
    }
}
