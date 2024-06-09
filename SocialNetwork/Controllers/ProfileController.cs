using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Services.Interfaces;
using System.Linq;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;

        public ProfileController(IUserService userService, IPostService postService)
        {
            _userService = userService;
            _postService = postService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            int userId = id ?? await _userService.GetUserIdFromClaimsAsync(User);
            var user = await _userService.GetUserProfileAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var posts = await _postService.GetUserPostsAsync(userId);

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Gender = user.Gender,
                Role = user.Role,
                Posts = posts.ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            var user = await _userService.GetUserProfileAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var model = new EditProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Gender = user.Gender,
                Email = user.Email,
                Username = user.Username
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userDto = new UserDTO
                {
                    Id = model.Id,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    ProfilePictureUrl = model.ProfilePictureUrl,
                    Gender = model.Gender,
                    Email = model.Email,
                    Username = model.Username
                };

                var result = await _userService.UpdateUserProfileAsync(userDto);
                if (result)
                {
                    return RedirectToAction(nameof(Index), new { id = model.Id });
                }

                ModelState.AddModelError(string.Empty, "An error occurred while updating the profile.");
            }

            return View(model);
        }
    }
}
