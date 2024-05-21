﻿namespace SocialNetwork.Models.Entity
{
    public class Friendship
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? AcceptanceDate { get; set; }

        public virtual UserProfile? User { get; set; }
        public virtual UserProfile? Friend { get; set; }

        public Friendship() { }
    }
}
