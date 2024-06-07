using SocialNetwork.Models;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetwork.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> AuthenticateUserAsync(string Username, string password);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<int> GetUserIdAsync(string Username);
        Task<UserDTO> GetUserProfileAsync(int userId);
        Task<IEnumerable<PostDTO>> GetUserPostsAsync(int userId);
        Task<bool> RegisterUserAsync(RegisterViewModel model);
        Task<bool> RegisterUserAsAdminAsync(string Username, string email, string password, Role role);
        Task<List<UserDTO>> SearchUsersAsync(string searchTerm);
        Task<bool> UpdateUserProfileAsync(UserDTO model);
        Task<bool> UpdateUserRoleAsync(int userId, Role newRole);
        Task<bool> ChangeUserRoleAsync(int userId, string role);
        Task<bool> BanUserAsync(int userId);
        Task<bool> UnbanUserAsync(int userId);
        Task<bool> CreateUserAsync(UserDTO User, string password);
    }
}
