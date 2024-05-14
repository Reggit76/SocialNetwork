namespace SocialNetwork.Models.Entity
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();
        public List<UserProfile> Participants { get; set; } = new List<UserProfile>();

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

        public Chat(string name, string description, List<UserProfile> participants)
        {
            Name = name;
            Description = description;
            Participants = participants;
        }

        // Методы для управления участниками
        public void AddParticipant(UserProfile user)
        {
            if (!Participants.Any(u => u.UserId == user.UserId))
            {
                Participants.Add(user);
            }
        }

        public void RemoveParticipant(UserProfile user)
        {
            Participants.RemoveAll(u => u.UserId == user.UserId);
        }

        // Метод для добавления сообщения в чат
        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }
    }
}
