using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Services.Interfaces;
using System.Threading.Tasks;

namespace SocialNetwork.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public ChatHub(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public async Task SendMessage(int chatId, string message)
        {
            var senderId = int.Parse(Context.User.FindFirst("UserId").Value);
            var userName = Context.User.Identity.Name;
            _messageService.SendMessageAsync(chatId, senderId, message);
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", userName, message);
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
