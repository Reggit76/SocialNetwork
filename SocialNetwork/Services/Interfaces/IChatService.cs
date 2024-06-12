using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface IChatService
    {
        Task<int> CreateChatAsync(string name, string description, List<int> participantIds, string chatIconUrl);
        Task<List<ChatDTO>> GetUserChatsAsync(int userId);
        Task<ChatDTO> GetChatAsync(int chatId);
        Task AddParticipantAsync(int chatId, int userId);
        Task RemoveParticipantAsync(int chatId, int userId);
        Task<List<MessageDTO>> GetMessagesAsync(int chatId);
        Task SendMessageAsync(int chatId, int senderId, string content);
        Task<List<UserDTO>> GetParticipantsAsync(int chatId);
        Task<bool> DeleteChatAsync(int chatId);
        Task<bool> UpdateChatAsync(int chatId, string name, string description, string chatIconUrl);
    }
}
