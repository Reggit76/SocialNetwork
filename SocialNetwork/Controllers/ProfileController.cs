using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Services.Interfaces;
using System.Linq;
using SocialNetwork.Models.DTO;
using Microsoft.Extensions.Logging;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IUserService userService, IPostService postService, ILogger<ProfileController> logger)
        {
            _userService = userService;
            _postService = postService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? id)
        {
            int userId = id ?? await _userService.GetUserIdFromClaimsAsync(User);
            _logger.LogInformation("Retrieving profile for user ID {userId}.", userId);

            var user = await _userService.GetUserProfileAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {userId} not found.", userId);
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

            _logger.LogInformation("Profile for user ID {userId} retrieved successfully.", userId);
            return View(model);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            _logger.LogInformation("Retrieving profile for editing for user ID {userId}.", userId);

            var user = await _userService.GetUserProfileAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {userId} not found.", userId);
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

            _logger.LogInformation("Profile for user ID {userId} prepared for editing.", userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Editing profile for user ID {userId}.", model.Id);

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
                    _logger.LogInformation("Profile for user ID {userId} updated successfully.", model.Id);
                    return RedirectToAction(nameof(Index), new { id = model.Id });
                }

                _logger.LogWarning("An error occurred while updating the profile for user ID {userId}.", model.Id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the profile.");
            }

            _logger.LogWarning("Model state is invalid for editing profile for user ID {userId}.", model.Id);
            return View(model);
        }
    }
}
