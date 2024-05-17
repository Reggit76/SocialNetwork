using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Models.Entity;
using SocialNetwork.Data;

public class MessageService
{
    private readonly UserDbContext _context;

    public MessageService(UserDbContext context)
    {
        _context = context;
    }

    public void SendMessage(int senderId, int chatId, string content)
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

    public List<Message> GetMessagesForUser(int userId)
    {
        return _context.Messages.Where(m => m.SenderId == userId).ToList();
    }
}
