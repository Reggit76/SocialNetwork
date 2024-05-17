namespace SocialNetwork.Models.Entity
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();
        public List<ChatUser> Participants { get; set; } = new List<ChatUser>();

        // Добавим конструктор для удобства создания чата
        public Chat()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        public Chat(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public Chat(string name, string description, List<ChatUser> participants)
        {
            Name = name;
            Description = description;
            Participants = participants;
        }
    }
}
