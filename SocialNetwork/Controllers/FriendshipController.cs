using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Services.Interfaces;
using System.Linq;
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
            var friendIds = friends.Select(f => f.Id).ToList();
            var allUsers = (await _userService.GetAllUsersAsync()).Where(u => !friendIds.Contains(u.Id) && u.Id != userId).ToList();
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



        [HttpPost]
        public async Task<IActionResult> SendRequest(int id)
        {
            var userId = await _userService.GetUserIdAsync(User.Identity.Name);
            if (userId == 0 || id == 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var result = await _friendshipService.SendFriendRequestAsync(userId, id);
            if (!result)
            {
                return BadRequest("Failed to send friend request.");
            }

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
