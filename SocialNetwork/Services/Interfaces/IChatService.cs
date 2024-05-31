using SocialNetwork.Models.DTO;

namespace SocialNetwork.Services.Interfaces
{
    public interface IChatService
    {
        void AddParticipant(int chatId, int userId);
        void CreateChat(string name, string description, List<int> participantIds);
        List<ChatDTO> GetUserChats(int userId);
        ChatDTO GetChat(int chatId);
        List<MessageDTO> GetMessages(int chatId);
        List<UserProfileDTO> GetParticipants(int chatId);
        void RemoveParticipant(int chatId, int userId);
        void SendMessage(int chatId, int senderId, string content);
    }
}