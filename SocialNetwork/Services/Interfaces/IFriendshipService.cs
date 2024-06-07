using System.Collections.Generic;
using SocialNetwork.Models;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface IFriendshipService
    {
        bool SendFriendRequest(int userId, int friendId);
        bool AcceptFriendRequest(int userId, int friendId);
        bool DeclineFriendRequest(int userId, int friendId);
        bool RemoveFriend(int userId, int friendId);
        List<UserDTO> GetFriends(int userId);
        List<UserDTO> GetPendingRequests(int userId);
        FriendshipStatus GetFriendshipStatus(int userId, int friendId);
    }
}
