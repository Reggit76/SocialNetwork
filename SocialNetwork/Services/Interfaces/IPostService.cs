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
        Task<bool> LikePostAsync(int postId);
        Task<bool> DislikePostAsync(int postId);
        Task<IEnumerable<PostDTO>> GetUserPostsAsync(int userId);
        Task<IEnumerable<PostDTO>> GetFriendsPostsAsync(int userId);
    }
}
