using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.Entity
{
    public class UserSession
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? LastActivity { get; set; }

        public UserSession(int userId)
        {
            UserId = userId;
            StartTime = DateTime.UtcNow;
            LastActivity = DateTime.UtcNow;
            Token = Guid.NewGuid().ToString(); // Генерация уникального идентификатора сессии
        }
    }
}
