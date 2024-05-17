namespace SocialNetwork.Models.Entity
{
    public class Friendship
    {
        public int Id { get; set; } 
        public int UserId { get; set; } 
        public int FriendId { get; set; } 
        public FriendshipStatus Status { get; set; } 
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
}
