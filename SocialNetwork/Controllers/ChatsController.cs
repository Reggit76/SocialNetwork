using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Services.Interfaces;
using SocialNetwork.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Hubs;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class ChatsController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ChatsController(IChatService chatService, IUserService userService, IHubContext<ChatHub> chatHubContext)
        {
            _chatService = chatService;
            _userService = userService;
            _chatHubContext = chatHubContext;
        }

        public async Task<IActionResult> Index()
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            var chats = _chatService.GetUserChats(userId);
            var allUsers = (await _userService.GetAllUsersAsync()).Where(u => u.Id != userId).ToList();
            ViewBag.AllUsers = allUsers;

            return View(chats);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateChatViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = await _userService.GetUserIdFromClaimsAsync(User);
                model.ParticipantIds.Add(userId); // Add the creator as a participant
                var isCreated = _chatService.CreateChat(model.Name, model.Description, model.ParticipantIds);
                if (isCreated)
                {
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }

        public IActionResult Details(int id)
        {
            var chat = _chatService.GetChat(id);
            if (chat == null)
            {
                return NotFound();
            }
            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatId, string message)
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            var userName = User.Identity.Name;
            _chatService.SendMessage(chatId, userId, message);
            await _chatHubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", userName, message);
            return Ok();
        }

        public async Task<IActionResult> JoinChat(int chatId)
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            _chatService.AddParticipant(chatId, userId);
            return RedirectToAction("Chat", new { id = chatId });
        }

        public IActionResult Chat(int id)
        {
            var chat = _chatService.GetChat(id);
            if (chat == null)
            {
                return NotFound();
            }
            var model = new ChatDetailViewModel
            {
                Chat = chat
            };
            return View(model);
        }
    }
}
