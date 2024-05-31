using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Services.Interfaces;
using System.Threading.Tasks;

namespace SocialNetwork.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessage(int chatId, int senderId, string content)
        {
            _messageService.SendMessage(chatId, senderId, content);
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", senderId, content);
        }

        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}
