using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Services.Interfaces;
using System.Linq;

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

        public IActionResult Index()
        {
            var userId = _userService.GetUserId(User.Identity.Name);
            var User = _userService.GetUserProfile(userId);
            var posts = _postService.GetUserPosts(userId).ToList();

            var model = new UserProfileViewModel
            {
                UserId = User.UserId,
                FullName = User.FullName,
                DateOfBirth = User.DateOfBirth,
                ProfilePictureUrl = User.ProfilePictureUrl,
                Gender = User.Gender,
                Role = User.Role,
                Posts = posts
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult EditProfile(int userId)
        {
            var User = _userService.GetUserProfile(userId);

            if (User == null)
            {
                return NotFound(); // Или любой другой обработчик ошибок
            }

            var model = new EditProfileViewModel
            {
                UserId = User.UserId,
                FullName = User.FullName,
                DateOfBirth = User.DateOfBirth,
                ProfilePictureUrl = User.ProfilePictureUrl,
                Gender = User.Gender,
                Role = User.Role
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var UserDTO = new UserDTO
                {
                    UserId = model.UserId,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    ProfilePictureUrl = model.ProfilePictureUrl,
                    Gender = model.Gender,
                    Role = model.Role
                };

                _userService.UpdateUserProfile(UserDTO);
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
