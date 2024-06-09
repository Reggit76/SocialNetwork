using SocialNetwork.Models.DTO;

namespace SocialNetwork.Models.ViewModels
{
    public class FriendshipViewModel
    {
        public List<UserDTO> Friends { get; set; }
        public List<UserDTO> AllUsers { get; set; }
        public List<UserDTO> IncomingRequests { get; set; } 
        public string SelectedTab { get; set; }

        public UserDTO SelectedUser { get; set; }  
        public FriendshipStatus FriendshipStatus { get; set; }  
    }
}
