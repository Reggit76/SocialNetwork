using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            var friends = _friendshipService.GetFriends(userId);
            var allUsers = _userService.GetAllUsers();
            var incomingRequests = _friendshipService.GetPendingRequests(userId);

            var model = new FriendshipViewModel
            {
                Friends = friends,
                AllUsers = allUsers,
                IncomingRequests = incomingRequests,
                SelectedTab = "Friends"
            };

            return View(model);
        }

        public IActionResult AllUsers()
        {
            var allUsers = _userService.GetAllUsers();
            return PartialView("_AllUsers", allUsers);
        }

        [HttpPost]
        public IActionResult SendRequest(int id)
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            _friendshipService.SendFriendRequest(userId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AcceptRequest(int id)
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            _friendshipService.AcceptFriendRequest(userId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeclineRequest(int id)
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            _friendshipService.DeclineFriendRequest(userId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFriend(int id)
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            _friendshipService.RemoveFriend(userId, id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetUserDetails(int id)
        {
            var user = _userService.GetUserProfile(id);
            var userId = _userService.GetUserId(User.Identity.Name);
            var friendshipStatus = _friendshipService.GetFriendshipStatus(userId, id);

            var model = new FriendshipViewModel
            {
                SelectedUser = user,
                FriendshipStatus = friendshipStatus
            };

            return PartialView("_UserDetails", model);
        }
    }
}
