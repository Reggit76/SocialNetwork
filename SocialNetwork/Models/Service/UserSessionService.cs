using SocialNetwork.Models.Entity;

namespace SocialNetwork.Models.Service
{
    public class UserSessionService
    {
        private readonly Dictionary<string, UserSession> _sessions = new Dictionary<string, UserSession>();

        // Создание новой сессии для пользователя
        public UserSession CreateSession(int userId)
        {
            var session = new UserSession(userId);
            _sessions.Add(session.Token, session);
            Console.WriteLine($"Session created: {session.Token} for user {userId}");
            return session;
        }

        // Получение сессии по идентификатору сессии
        public UserSession GetSession(string Token)
        {
            if (_sessions.TryGetValue(Token, out UserSession session))
            {
                // Обновление времени последней активности
                session.LastActivity = DateTime.UtcNow;
                return session;
            }
            return null;
        }

        // Завершение сессии
        public bool TerminateSession(string Token)
        {
            if (_sessions.Remove(Token))
            {
                Console.WriteLine($"Session terminated: {Token}");
                return true;
            }
            return false;
        }

        // Проверка активности сессии
        public bool IsSessionActive(string Token)
        {
            if (_sessions.TryGetValue(Token, out UserSession session))
            {
                return (DateTime.UtcNow - session.LastActivity)?.Hours < 24; // Сессия активна, если последняя активность была менее 30 минут назад
            }
            return false;
        }
    }
}
