using SocialNetwork.Models.DTO;

namespace SocialNetwork.Models.ViewModels
{
    public class NewsViewModel
    {
        public List<PostDTO> Posts { get; set; }
        public string Filter { get; set; }
    }
}
