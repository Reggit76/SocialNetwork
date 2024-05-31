using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface IMessageService
    {
        List<MessageDTO> GetMessages(int userId);
        void SendMessage(int senderId, int chatId, string content);
    }
}