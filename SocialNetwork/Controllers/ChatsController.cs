using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Services.Interfaces;
using SocialNetwork.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Hubs;
using Microsoft.Extensions.Logging;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class ChatsController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly ILogger<ChatsController> _logger;

        public ChatsController(IChatService chatService, IUserService userService, IHubContext<ChatHub> chatHubContext, ILogger<ChatsController> logger)
        {
            _chatService = chatService;
            _userService = userService;
            _chatHubContext = chatHubContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            var chats = await _chatService.GetUserChatsAsync(userId);
            var allUsers = (await _userService.GetAllUsersAsync()).Where(u => u.Id != userId).ToList();
            ViewBag.AllUsers = allUsers;

            _logger.LogInformation("User ID {userId} retrieved from claims.", userId);
            _logger.LogInformation("{chatCount} chats retrieved for user ID {userId}.", chats.Count, userId);
            _logger.LogInformation("{userCount} users retrieved for chat creation, excluding current user.", allUsers.Count);

            return View(chats);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateChatViewModel model)
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            _logger.LogInformation("User ID {userId} retrieved from claims for chat creation.", userId);

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model state is valid for chat creation.");

                model.ParticipantIds.Add(userId); // Add the creator as a participant
                var success = await _chatService.CreateChatAsync(model.Name, model.Description, model.ParticipantIds);

                _logger.LogInformation("Chat creation status: {success}", success);

                return Json(new { success });
            }

            _logger.LogWarning("Model state is invalid for chat creation.");
            return Json(new { success = false });
        }

        public async Task<IActionResult> Details(int id)
        {
            var chat = await _chatService.GetChatAsync(id);
            if (chat == null)
            {
                _logger.LogWarning("Chat with ID {chatId} not found.", id);
                return NotFound();
            }
            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatId, string message)
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            var userName = User.Identity.Name;

            _logger.LogInformation("User ID {userId} is sending message in chat ID {chatId}.", userId, chatId);

            await _chatService.SendMessageAsync(chatId, userId, message);
            await _chatHubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", userName, message);

            return Ok();
        }

        public async Task<IActionResult> JoinChat(int chatId)
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            _logger.LogInformation("User ID {userId} is joining chat ID {chatId}.", userId, chatId);

            await _chatService.AddParticipantAsync(chatId, userId);
            return RedirectToAction("Chat", new { id = chatId });
        }

        public async Task<IActionResult> Chat(int id)
        {
            var chat = await _chatService.GetChatAsync(id);
            if (chat == null)
            {
                _logger.LogWarning("Chat with ID {chatId} not found.", id);
                return NotFound();
            }
            var model = new ChatDetailViewModel
            {
                Chat = chat
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _chatService.DeleteChatAsync(id);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditChatViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _chatService.UpdateChatAsync(model.Id, model.Name, model.Description, model.ChatIconUrl);
                if (success)
                {
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
            }
            return BadRequest();
        }
    }
}
