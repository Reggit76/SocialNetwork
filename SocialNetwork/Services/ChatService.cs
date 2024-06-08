using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Models.DTO;
using SocialNetwork.Data;
using SocialNetwork.Services.Interfaces;

namespace SocialNetwork.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMessageService _messageService;

        public ChatService(ApplicationDbContext context, IMessageService messageService)
        {
            _context = context;
            _messageService = messageService;
        }

        public void CreateChat(string name, string description, List<int> participantIds)
        {
            var chat = new Chat
            {
                Name = name,
                Description = description,
                Participants = participantIds.Select(id => new ChatUser { UserId = id }).ToList()
            };

            _context.Chats.Add(chat);
            _context.SaveChanges();
        }

        public List<ChatDTO> GetUserChats(int Id)
        {
            return _context.ChatUsers
                .Where(cu => cu.UserId == Id)
                .Include(cu => cu.Chat)
                .ThenInclude(c => c.Messages)
                .Include(cu => cu.Chat)
                .ThenInclude(c => c.Participants)
                .ThenInclude(cp => cp.User)
                .Select(cu => new ChatDTO
                {
                    Id = cu.Chat.Id,
                    Name = cu.Chat.Name,
                    Description = cu.Chat.Description,
                    Participants = cu.Chat.Participants.Select(p => new UserDTO
                    {
                        Id = p.User.Id,
                        FullName = p.User.FullName,
                        Gender = p.User.Gender,
                        DateOfBirth = p.User.DateOfBirth,
                        ProfilePictureUrl = p.User.ProfilePictureUrl,
                        Role = p.User.Role
                    }).ToList(),
                    Messages = cu.Chat.Messages.Select(m => new MessageDTO
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        ChatId = m.ChatId,
                        Content = m.Content,
                        Timestamp = m.Timestamp
                    }).ToList()
                }).ToList();
        }

        public ChatDTO GetChat(int chatId)
        {
            var chat = _context.Chats
                .Include(c => c.Messages)
                .Include(c => c.Participants)
                .ThenInclude(cp => cp.User)
                .FirstOrDefault(c => c.Id == chatId);

            if (chat == null)
                return null;

            return new ChatDTO
            {
                Id = chat.Id,
                Name = chat.Name,
                Description = chat.Description,
                Participants = chat.Participants.Select(p => new UserDTO
                {
                    Id = p.User.Id,
                    FullName = p.User.FullName,
                    Gender = p.User.Gender,
                    DateOfBirth = p.User.DateOfBirth,
                    ProfilePictureUrl = p.User.ProfilePictureUrl,
                    Role = p.User.Role
                }).ToList(),
                Messages = chat.Messages.Select(m => new MessageDTO
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ChatId = m.ChatId,
                    Content = m.Content,
                    Timestamp = m.Timestamp
                }).ToList()
            };
        }

        public void AddParticipant(int chatId, int Id)
        {
            var chatUser = new ChatUser
            {
                ChatId = chatId,
                UserId = Id
            };

            _context.ChatUsers.Add(chatUser);
            _context.SaveChanges();
        }

        public void RemoveParticipant(int chatId, int Id)
        {
            var chatUser = _context.ChatUsers.FirstOrDefault(cu => cu.ChatId == chatId && cu.UserId == Id);
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

        public List<UserDTO> GetParticipants(int chatId)
        {
            return _context.ChatUsers
                .Where(cu => cu.ChatId == chatId)
                .Include(cu => cu.User)
                .Select(cu => new UserDTO
                {
                    Id = cu.User.Id,
                    FullName = cu.User.FullName,
                    Gender = cu.User.Gender,
                    DateOfBirth = cu.User.DateOfBirth,
                    ProfilePictureUrl = cu.User.ProfilePictureUrl,
                    Role = cu.User.Role
                }).ToList();
        }
    }
}
