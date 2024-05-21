﻿namespace SocialNetwork.Models.Entity
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Timestamp { get; set; }

        public virtual Chat? Chat { get; set; }
        public virtual UserProfile? Sender { get; set; }

        public Message() { }
    }
}
