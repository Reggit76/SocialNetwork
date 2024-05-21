namespace SocialNetwork.Models.DTO
{
    public class ChatDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<MessageDTO> Messages { get; set; } = new List<MessageDTO>();
        public List<UserProfileDTO> Participants { get; set; } = new List<UserProfileDTO>();
    }
}
