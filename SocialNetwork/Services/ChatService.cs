using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Data;

namespace SocialNetwork.Services
{
    public class ChatService
    {
        private readonly UserDbContext _context;

        public ChatService(UserDbContext context)
        {
            _context = context;
        }

        public Chat CreateChat(string name, string description, List<int> participantIds)
        {
            var chat = new Chat
            {
                Name = name,
                Description = description,
            };

            _context.Chats.Add(chat);
            _context.SaveChanges();

            // Добавление участников в чат
            foreach (var userId in participantIds)
            {
                var chatUser = new ChatUser
                {
                    ChatId = chat.Id,
                    UserId = userId
                };
                _context.ChatUsers.Add(chatUser);
            }

            _context.SaveChanges();
            return chat;
        }

        public void AddParticipant(int chatId, int userId)
        {
            var chatUser = new ChatUser
            {
                ChatId = chatId,
                UserId = userId
            };

            _context.ChatUsers.Add(chatUser);
            _context.SaveChanges();
        }

        public void RemoveParticipant(int chatId, int userId)
        {
            var chatUser = _context.ChatUsers.FirstOrDefault(cu => cu.ChatId == chatId && cu.UserId == userId);
            if (chatUser != null)
            {
                _context.ChatUsers.Remove(chatUser);
                _context.SaveChanges();
            }
        }

        public List<Message> GetMessages(int chatId)
        {
            return _context.Messages.Where(m => m.ChatId == chatId).ToList();
        }

        public void SendMessage(int chatId, int senderId, string content)
        {
            var chat = _context.Chats.FirstOrDefault(c => c.Id == chatId);
            if (chat != null)
            {
                var message = new Message
                {
                    SenderId = senderId,
                    ChatId = chatId,
                    Content = content
                };

                _context.Messages.Add(message);
                _context.SaveChanges();
            }
        }

        public List<UserProfile> GetParticipants(int chatId)
        {
            return _context.ChatUsers
                .Where(cu => cu.ChatId == chatId)
                .Include(cu => cu.UserProfile)
                .Select(cu => cu.UserProfile)
                .ToList();
        }
    }
}