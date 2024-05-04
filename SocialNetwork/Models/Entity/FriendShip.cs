namespace SocialNetwork.Models.Entity
{
    public class Friendship
    {
        public int Id { get; set; } // Уникальный идентификатор дружбы
        public int UserId { get; set; } // Идентификатор пользователя, отправившего запрос
        public int FriendId { get; set; } // Идентификатор пользователя, получившего запрос
        public FriendshipStatus Status { get; set; } // Статус дружбы
        public DateTime RequestDate { get; set; } // Дата отправки запроса
        public DateTime? AcceptanceDate { get; set; } // Дата принятия запроса, если применимо

        public Friendship()
        {
            RequestDate = DateTime.UtcNow;
            Status = FriendshipStatus.Pending;
        }

        public Friendship(int userId, int friendId) : this()
        {
            UserId = userId;
            FriendId = friendId;
        }
    }

    public enum FriendshipStatus
    {
        Pending, // Запрос отправлен, но еще не принят
        Accepted, // Запрос принят, дружба установлена
        Declined // Запрос отклонен
    }
}
