namespace SocialNetwork.Models.DTO
{
    public class FriendshipDTO
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? AcceptanceDate { get; set; }
    }
}
