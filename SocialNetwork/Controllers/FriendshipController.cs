using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Services.Interfaces;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class FriendshipController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IUserService _userService;

        public FriendshipController(IFriendshipService friendshipService, IUserService userService)
        {
            _friendshipService = friendshipService;
            _userService = userService;
        }

        public IActionResult Index(string searchTerm = null, string tab = "Friends")
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            var friends = _friendshipService.GetFriends(userId);
            var allUsers = _userService.SearchUsers(searchTerm);
            var incomingRequests = _friendshipService.GetIncomingFriendRequests(userId); // Получение входящих запросов

            var model = new FriendshipViewModel
            {
                Friends = friends,
                AllUsers = allUsers,
                IncomingRequests = incomingRequests, // Добавлено в модель
                SearchTerm = searchTerm,
                SelectedTab = tab
            };

            return View(model);
        }

        public IActionResult UserDetails(int userId, string tab = "Friends")
        {
            var currentUserId = _userService.GetUserId(User.Identity.Name);
            var user = _userService.GetUserProfile(userId);

            var friends = _friendshipService.GetFriends(currentUserId);
            var allUsers = _userService.SearchUsers(null);
            var incomingRequests = _friendshipService.GetIncomingFriendRequests(currentUserId); // Получение входящих запросов

            var model = new FriendshipViewModel
            {
                Friends = friends,
                AllUsers = allUsers,
                IncomingRequests = incomingRequests, // Добавлено в модель
                SelectedUser = user,
                IsFriend = _friendshipService.IsFriend(currentUserId, userId),
                HasPendingRequest = _friendshipService.HasPendingRequest(currentUserId, userId),
                SelectedTab = tab
            };

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult SendFriendRequest(int friendId)
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            _friendshipService.SendFriendRequest(userId, friendId);
            return RedirectToAction("UserDetails", new { userId = friendId });
        }

        [HttpPost]
        public IActionResult AcceptFriendRequest(int friendId)
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            _friendshipService.AcceptFriendRequest(userId, friendId);
            return RedirectToAction("UserDetails", new { userId = friendId });
        }

        [HttpPost]
        public IActionResult RemoveFriend(int friendId)
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            _friendshipService.RemoveFriend(userId, friendId);
            return RedirectToAction("UserDetails", new { userId = friendId });
        }

        public IActionResult PendingRequests()
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            var pendingRequests = _friendshipService.GetPendingRequests(userId);
            return View(pendingRequests);
        }
    }
}
