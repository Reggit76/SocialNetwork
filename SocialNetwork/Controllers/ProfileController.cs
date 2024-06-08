using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            var userId = await _userService.GetUserIdAsync(User.Identity.Name);
            var user = await _userService.GetUserProfileAsync(userId);
            var posts = (await _postService.GetUserPostsAsync(userId)).ToList();

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Gender = user.Gender,
                Role = user.Role,
                Posts = posts
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile(int userId)
        {
            var user = await _userService.GetUserProfileAsync(userId);

            if (user == null)
            {
                return NotFound(); // Или любой другой обработчик ошибок
            }

            var model = new EditProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Gender = user.Gender,
                Role = user.Role,
                Username = user.Username,
                Email = user.Email,
                AvatarUrl = user.ProfilePictureUrl,
                Description = user.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userDTO = new UserDTO
                {
                    Id = model.Id,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    ProfilePictureUrl = model.ProfilePictureUrl,
                    Gender = model.Gender,
                    Role = model.Role,
                    Username = model.Username,
                    Email = model.Email,
                    Description = model.Description
                };

                await _userService.UpdateUserProfileAsync(userDTO);
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
