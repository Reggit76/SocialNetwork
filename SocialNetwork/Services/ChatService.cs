using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Models.DTO;
using SocialNetwork.Data;

namespace SocialNetwork.Services
{
    public class ChatService
    {
        private readonly UserDbContext _context;
        private readonly MessageService _messageService;

        public ChatService(UserDbContext context, MessageService messageService)
        {
            _context = context;
            _messageService = messageService;
        }

        public ChatDTO CreateChat(string name, string description, List<int> participantIds)
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

            return new ChatDTO
            {
                Id = chat.Id,
                Name = chat.Name,
                Description = chat.Description,
                Participants = participantIds.Select(id => new UserProfileDTO { UserId = id }).ToList(),
                Messages = new List<MessageDTO>()
            };
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

        public List<MessageDTO> GetMessages(int chatId)
        {
            return _context.Messages
                .Where(m => m.ChatId == chatId)
                .Select(m => new MessageDTO
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ChatId = m.ChatId,
                    Content = m.Content,
                    Timestamp = m.Timestamp
                }).ToList();
        }

        public void SendMessage(int chatId, int senderId, string content)
        {
            _messageService.SendMessage(senderId, chatId, content);
        }

        public List<UserProfileDTO> GetParticipants(int chatId)
        {
            return _context.ChatUsers
                .Where(cu => cu.ChatId == chatId)
                .Include(cu => cu.UserProfile)
                .Select(cu => new UserProfileDTO
                {
                    UserId = cu.UserProfile.UserId,
                    FullName = cu.UserProfile.FullName,
                    Gender = cu.UserProfile.Gender,
                    DateOfBirth = cu.UserProfile.DateOfBirth,
                    ProfilePictureUrl = cu.UserProfile.ProfilePictureUrl,
                    Role = cu.UserProfile.Role
                }).ToList();
        }
    }
}
