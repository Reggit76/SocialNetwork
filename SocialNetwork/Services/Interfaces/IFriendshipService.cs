using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface IFriendshipService
    {
        bool AcceptFriendRequest(int userId, int friendId);
        List<UserProfileDTO> GetFriends(int userId);
        List<UserProfileDTO> GetIncomingFriendRequests(int userId);
        List<FriendshipDTO> GetPendingRequests(int userId);
        bool HasPendingRequest(int userId, int friendId);
        bool IsFriend(int userId, int friendId);
        bool RemoveFriend(int userId, int friendId);
        bool SendFriendRequest(int userId, int friendId);
    }
}