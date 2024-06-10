namespace SocialNetwork.Models.DTO
{
    public class ChatDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ChatIconUrl { get; set; } // Add this line
        public List<UserDTO> Participants { get; set; } = new List<UserDTO>();
        public List<MessageDTO> Messages { get; set; } = new List<MessageDTO>();
    }
}
