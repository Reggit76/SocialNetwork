using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Data;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.Entity;
using SocialNetwork.Services.Interfaces;

namespace SocialNetwork.Services
{
    public class MessageService : IMessageService
    {
        private readonly UserDbContext _context;

        public MessageService(UserDbContext context)
        {
            _context = context;
        }

        public void SendMessage(int chatId, int senderId, string content)
        {
            var message = new Message
            {
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            _context.SaveChanges();
        }

        public List<MessageDTO> GetMessages(int chatId)
        {
            return _context.Messages
                .Where(m => m.ChatId == chatId)
                .Select(m => new MessageDTO
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    Timestamp = m.Timestamp
                }).ToList();
        }
    }
}
