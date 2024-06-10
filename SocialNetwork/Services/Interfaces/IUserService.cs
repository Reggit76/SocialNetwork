using SocialNetwork.Models;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialNetwork.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> AuthenticateUserAsync(string Username, string password);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<int> GetUserIdAsync(string Username);
        Task<int> GetUserIdFromClaimsAsync(ClaimsPrincipal userClaims);
        Task<UserDTO> GetUserProfileAsync(int userId);
        Task<IEnumerable<PostDTO>> GetUserPostsAsync(int userId);
        Task<bool> RegisterUserAsync(RegisterViewModel model);
        Task<bool> RegisterUserAsAdminAsync(string Username, string email, string password, Role role);
        Task<List<UserDTO>> SearchUsersAsync(string searchTerm);
        Task<bool> UpdateUserProfileAsync(UserDTO model);
        Task<bool> ChangeUserRoleAsync(int userId, string role);
        Task<bool> BanUserAsync(int userId);
        Task<bool> UnbanUserAsync(int userId);
        Task<bool> CreateUserAsync(UserDTO User, string password);
        Task<string> GetUserRoleAsync(string username);
        Task<List<UserDTO>> GetAllUsersExceptFriendsAsync(int userId, List<int> friendIds);
    }
}
