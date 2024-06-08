using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Services.Interfaces;
using System.Linq;
using System.Security.Claims;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public ChatController(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = await _userService.GetUserIdAsync(User.Identity.Name);
            var chats = _chatService.GetUserChats(userId);
            return View(chats);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ChatViewModel model)
        {
            if (ModelState.IsValid)
            {
                var participantIds = model.Participants.Split(',')
                    .Select(id => int.Parse(id)).ToList();

                _chatService.CreateChat(model.Name, model.Description, participantIds);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var chat = _chatService.GetChat(id);
            return View(chat);
        }
    }
}
