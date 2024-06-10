using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Models.DTO;
using SocialNetwork.Data;
using SocialNetwork.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace SocialNetwork.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMessageService _messageService;
        private readonly ILogger<ChatService> _logger;

        public ChatService(ApplicationDbContext context, IMessageService messageService, ILogger<ChatService> logger)
        {
            _context = context;
            _messageService = messageService;
            _logger = logger;
        }

        public async Task<bool> CreateChatAsync(string name, string description, List<int> participantIds)
        {
            _logger.LogInformation("Attempting to create chat with name: {name}, description: {description}, and participants: {participantIds}", name, description, string.Join(",", participantIds));

            var chat = new Chat
            {
                Name = name,
                Description = description,
                Participants = participantIds.Select(id => new ChatUser { UserId = id }).ToList()
            };

            await _context.Chats.AddAsync(chat);
            var result = await _context.SaveChangesAsync();
            var isSuccess = result > 0;
            _logger.LogInformation("Chat creation {status}. Result: {result}", isSuccess ? "successful" : "failed", result);

            return isSuccess;
        }

        public async Task<List<ChatDTO>> GetUserChatsAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Retrieving chats for user ID {userId}", userId);

                var chats = await _context.ChatUsers
                    .Where(cu => cu.UserId == userId)
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
                    }).ToListAsync();
                _logger.LogInformation("Retrieved {chatCount} chats for user ID {userId}.", chats.Count, userId);
                return chats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user chats.");
                return new List<ChatDTO>();
            }
        }

        public async Task<ChatDTO> GetChatAsync(int chatId)
        {
            try
            {
                _logger.LogInformation("Retrieving chat with ID {chatId}", chatId);

                var chat = await _context.Chats
                    .Include(c => c.Messages)
                    .Include(c => c.Participants)
                    .ThenInclude(cp => cp.User)
                    .FirstOrDefaultAsync(c => c.Id == chatId);

                if (chat == null)
                {
                    _logger.LogWarning("Chat with ID {chatId} not found.", chatId);
                    return null;
                }

                var chatDTO = new ChatDTO
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
                _logger.LogInformation("Retrieved chat with ID {chatId}.", chatId);
                return chatDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving chat with ID {chatId}.", chatId);
                return null;
            }
        }

        public async Task AddParticipantAsync(int chatId, int userId)
        {
            try
            {
                _logger.LogInformation("Adding participant with user ID {userId} to chat with ID {chatId}", userId, chatId);

                var chatUser = new ChatUser
                {
                    ChatId = chatId,
                    UserId = userId
                };

                await _context.ChatUsers.AddAsync(chatUser);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User ID {userId} added to chat ID {chatId}.", userId, chatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding user ID {userId} to chat ID {chatId}.", userId, chatId);
            }
        }

        public async Task RemoveParticipantAsync(int chatId, int userId)
        {
            try
            {
                _logger.LogInformation("Removing participant with user ID {userId} from chat with ID {chatId}", userId, chatId);

                var chatUser = await _context.ChatUsers.FirstOrDefaultAsync(cu => cu.ChatId == chatId && cu.UserId == userId);
                if (chatUser != null)
                {
                    _context.ChatUsers.Remove(chatUser);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User ID {userId} removed from chat ID {chatId}.", userId, chatId);
                }
                else
                {
                    _logger.LogWarning("User ID {userId} not found in chat ID {chatId}.", userId, chatId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing user ID {userId} from chat ID {chatId}.", userId, chatId);
            }
        }

        public async Task<List<MessageDTO>> GetMessagesAsync(int chatId)
        {
            try
            {
                _logger.LogInformation("Retrieving messages for chat with ID {chatId}", chatId);

                var messages = await _context.Messages
                    .Where(m => m.ChatId == chatId)
                    .Select(m => new MessageDTO
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        ChatId = m.ChatId,
                        Content = m.Content,
                        Timestamp = m.Timestamp
                    }).ToListAsync();
                _logger.LogInformation("Retrieved {messageCount} messages for chat ID {chatId}.", messages.Count, chatId);
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving messages for chat ID {chatId}.", chatId);
                return new List<MessageDTO>();
            }
        }

        public async Task SendMessageAsync(int chatId, int senderId, string content)
        {
            try
            {
                await _messageService.SendMessageAsync(chatId, senderId, content);
                _logger.LogInformation("Message sent in chat ID {chatId} by user ID {senderId}.", chatId, senderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending message in chat ID {chatId} by user ID {senderId}.", chatId, senderId);
            }
        }

        public async Task<List<UserDTO>> GetParticipantsAsync(int chatId)
        {
            try
            {
                _logger.LogInformation("Retrieving participants for chat with ID {chatId}", chatId);

                var participants = await _context.ChatUsers
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
                    }).ToListAsync();
                _logger.LogInformation("Retrieved {participantCount} participants for chat ID {chatId}.", participants.Count, chatId);
                return participants;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving participants for chat ID {chatId}.", chatId);
                return new List<UserDTO>();
            }
        }

        public async Task<bool> DeleteChatAsync(int chatId)
        {
            try
            {
                _logger.LogInformation("Attempting to delete chat with ID {chatId}", chatId);

                var chat = await _context.Chats.FindAsync(chatId);
                if (chat == null)
                {
                    _logger.LogWarning("Chat with ID {chatId} not found.", chatId);
                    return false;
                }

                _context.Chats.Remove(chat);
                var result = await _context.SaveChangesAsync();
                var isSuccess = result > 0;
                _logger.LogInformation("Chat deletion {status}. Result: {result}", isSuccess ? "successful" : "failed", result);

                return isSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting chat with ID {chatId}.", chatId);
                return false;
            }
        }

        public async Task<bool> UpdateChatAsync(int chatId, string name, string description, string chatIconUrl)
        {
            try
            {
                _logger.LogInformation("Attempting to update chat with ID {chatId}", chatId);

                var chat = await _context.Chats.FindAsync(chatId);
                if (chat == null)
                {
                    _logger.LogWarning("Chat with ID {chatId} not found.", chatId);
                    return false;
                }

                chat.Name = name;
                chat.Description = description;
                chat.ChatIconUrl = chatIconUrl;

                _context.Chats.Update(chat);
                var result = await _context.SaveChangesAsync();
                var isSuccess = result > 0;
                _logger.LogInformation("Chat update {status}. Result: {result}", isSuccess ? "successful" : "failed", result);

                return isSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating chat with ID {chatId}.", chatId);
                return false;
            }
        }
    }
}
