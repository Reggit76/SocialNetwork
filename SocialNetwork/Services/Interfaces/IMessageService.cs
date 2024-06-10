using SocialNetwork.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetwork.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageDTO>> GetMessagesAsync(int chatId);
        Task SendMessageAsync(int senderId, int chatId, string content);
    }
}
