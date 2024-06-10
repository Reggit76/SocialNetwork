using SocialNetwork.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetwork.Services.Interfaces
{
    public interface IChatService
    {
        Task AddParticipantAsync(int chatId, int userId);
        Task<bool> CreateChatAsync(string name, string description, List<int> participantIds);
        Task<List<ChatDTO>> GetUserChatsAsync(int userId);
        Task<ChatDTO> GetChatAsync(int chatId);
        Task<List<MessageDTO>> GetMessagesAsync(int chatId);
        Task<List<UserDTO>> GetParticipantsAsync(int chatId);
        Task RemoveParticipantAsync(int chatId, int userId);
        Task SendMessageAsync(int chatId, int senderId, string content);
        Task<bool> DeleteChatAsync(int chatId);
        Task<bool> UpdateChatAsync(int chatId, string name, string description, string chatIconUrl);
    }
}
