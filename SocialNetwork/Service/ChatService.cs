using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Models.Entity;
using SocialNetwork.Data;
using Microsoft.EntityFrameworkCore;

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
            Participants = _context.UserProfiles.Where(up => participantIds.Contains(up.UserId)).ToList()
        };

        _context.Chats.Add(chat);
        _context.SaveChanges();
        return chat;
    }

    public void AddParticipant(int chatId, int userId)
    {
        var chat = _context.Chats.Include(c => c.Participants).FirstOrDefault(c => c.Id == chatId);
        var user = _context.UserProfiles.FirstOrDefault(u => u.UserId == userId);
        if (chat != null && user != null && !chat.Participants.Contains(user))
        {
            chat.Participants.Add(user);
            _context.SaveChanges();
        }
    }

    public void RemoveParticipant(int chatId, int userId)
    {
        var chat = _context.Chats.Include(c => c.Participants).FirstOrDefault(c => c.Id == chatId);
        var user = _context.UserProfiles.FirstOrDefault(u => u.UserId == userId);
        if (chat != null && user != null && chat.Participants.Contains(user))
        {
            chat.Participants.Remove(user);
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
                Content = content,
                TimeStamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            _context.SaveChanges();
        }
    }
}
