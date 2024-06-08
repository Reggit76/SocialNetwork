using SocialNetwork.Models;
using SocialNetwork.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetwork.Services.Interfaces
{
    public interface IFriendshipService
    {
        Task<bool> SendFriendRequestAsync(int userId, int friendId);
        Task<bool> AcceptFriendRequestAsync(int userId, int friendId);
        Task<bool> DeclineFriendRequestAsync(int userId, int friendId);
        Task<bool> RemoveFriendAsync(int userId, int friendId);
        Task<List<UserDTO>> GetFriendsAsync(int userId);
        Task<List<UserDTO>> GetPendingRequestsAsync(int userId);
        Task<FriendshipStatus> GetFriendshipStatusAsync(int userId, int friendId);
    }
}
