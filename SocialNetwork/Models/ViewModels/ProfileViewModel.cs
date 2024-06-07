using SocialNetwork.Models.DTO;

namespace SocialNetwork.Models.ViewModels
{
    public class ProfileViewModel
    {
        public UserDTO User { get; set; }
        public List<PostDTO> Posts { get; set; }
    }
}
