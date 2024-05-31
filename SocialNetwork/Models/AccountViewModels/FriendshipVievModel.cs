using System.Collections.Generic;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Models.ViewModels
{
    public class FriendshipViewModel
    {
        public List<UserProfileDTO> Friends { get; set; }
        public List<UserProfileDTO> AllUsers { get; set; }
        public UserProfileDTO SelectedUser { get; set; }
        public bool IsFriend { get; set; }
        public bool HasPendingRequest { get; set; }
        public string SearchTerm { get; set; }
        public string SelectedTab { get; set; }
        public List<UserProfileDTO> IncomingRequests { get; set; } // Добавлено для входящих запросов
    }
}
