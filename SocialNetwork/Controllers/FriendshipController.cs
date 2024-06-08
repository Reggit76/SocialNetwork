using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Services.Interfaces;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            var userId = await _userService.GetUserIdAsync(User.Identity.Name);
            var friends = await _friendshipService.GetFriendsAsync(userId);
            var allUsers = await _userService.GetAllUsersAsync();
            var incomingRequests = await _friendshipService.GetPendingRequestsAsync(userId);

            var model = new FriendshipViewModel
            {
                Friends = friends,
                AllUsers = allUsers,
                IncomingRequests = incomingRequests,
                SelectedTab = "Friends"
            };

            return View(model);
        }

        public async Task<IActionResult> AllUsers()
        {
            var allUsers = await _userService.GetAllUsersAsync();
            return PartialView("_AllUsers", allUsers);
        }

        [HttpPost]
        public async Task<IActionResult> SendRequest(int id)
        {
            var userId = await _userService.GetUserIdAsync(User.Identity.Name);
            await _friendshipService.SendFriendRequestAsync(userId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            var userId = await _userService.GetUserIdAsync(User.Identity.Name);
            await _friendshipService.AcceptFriendRequestAsync(userId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeclineRequest(int id)
        {
            var userId = await _userService.GetUserIdAsync(User.Identity.Name);
            await _friendshipService.DeclineFriendRequestAsync(userId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int id)
        {
            var userId = await _userService.GetUserIdAsync(User.Identity.Name);
            await _friendshipService.RemoveFriendAsync(userId, id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            var user = await _userService.GetUserProfileAsync(id);
            var userId = await _userService.GetUserIdAsync(User.Identity.Name);
            var friendshipStatus = await _friendshipService.GetFriendshipStatusAsync(userId, id);

            var model = new FriendshipViewModel
            {
                SelectedUser = user,
                FriendshipStatus = friendshipStatus
            };

            return PartialView("_UserDetails", model);
        }
    }
}
