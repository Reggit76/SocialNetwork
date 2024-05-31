using SocialNetwork.Models;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface IUserService
    {
        UserDTO AuthenticateUser(string username, string password);
        List<UserDTO> GetAllUsers();
        List<ChatDTO> GetUserChats(int userId);
        int GetUserId(string username);
        UserProfileDTO GetUserProfile(int userId);
        UserRole GetUserRole(int userId);
        bool RegisterUser(string username, string email, string password);
        bool RegisterUserAsAdmin(string username, string email, string password, UserRole role);
        List<UserProfileDTO> SearchUsers(string searchTerm);
        bool UpdateUserProfile(UserProfileDTO profile);
        void UpdateUserRole(int userId, UserRole newRole);
    }
}