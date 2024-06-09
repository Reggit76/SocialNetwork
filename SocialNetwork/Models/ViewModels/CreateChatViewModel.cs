namespace SocialNetwork.Models.ViewModels
{
    public class CreateChatViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> ParticipantIds { get; set; } = new List<int>();
    }
}
