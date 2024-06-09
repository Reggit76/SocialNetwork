using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Services.Interfaces;
using SocialNetwork.Models;
using SocialNetwork.Models.Entity;

namespace SocialNetwork.Controllers
{
    [Authorize(Roles = "Administrator, Moderator")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(string search)
        {
            var users = await _userService.GetAllUsersAsync();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.Username.Contains(search) || u.Email.Contains(search)).ToList();
            }

            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserProfileAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var model = new EditProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Gender = user.Gender,
                AvatarUrl = user.ProfilePictureUrl,
                Description = user.Description,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserDTO
                {
                    Id = model.Id,
                    Username = model.Username,
                    Email = model.Email,
                    Gender = model.Gender,
                    ProfilePictureUrl = model.AvatarUrl,
                    Description = model.Description,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth
                };

                var result = await _userService.UpdateUserProfileAsync(user);
                if (result)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "An error occurred while updating the profile.");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ChangeRole(int id, string role)
        {
            var user = await _userService.GetUserProfileAsync(id);
            if (user == null || user.Role == Role.Administrator)
            {
                return BadRequest("Invalid user or role.");
            }

            if ((role == "Administrator" && User.IsInRole("Administrator") && user.Role != Role.Administrator) ||
                (role == "Moderator" && User.IsInRole("Administrator") && user.Role != Role.Administrator) ||
                (role == "RegularUser" && User.IsInRole("Administrator") && user.Role != Role.Administrator))
            {
                await _userService.ChangeUserRoleAsync(id, role);
            }
            else
            {
                return Forbid();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> BanUser(int id)
        {
            var user = await _userService.GetUserProfileAsync(id);
            if (user == null || user.Role == Role.Administrator)
            {
                return BadRequest("Invalid user.");
            }

            if (User.IsInRole("Administrator") || (User.IsInRole("Moderator") && user.Role == Role.RegularUser))
            {
                await _userService.BanUserAsync(id);
            }
            else
            {
                return Forbid();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> UnbanUser(int id)
        {
            var user = await _userService.GetUserProfileAsync(id);
            if (user == null)
            {
                return BadRequest("Invalid user.");
            }

            await _userService.UnbanUserAsync(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserDTO
                {
                    Username = model.Username,
                    Email = model.Email,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    ProfilePictureUrl = string.IsNullOrEmpty(model.ProfilePictureUrl) ? "~/Images/default-avatar.jpg" : model.ProfilePictureUrl,
                    Description = model.Description,
                    Role = model.Role,
                    FullName = model.FullName
                };

                var result = await _userService.CreateUserAsync(user, model.Password);
                if (result)
                {
                    await _userService.ChangeUserRoleAsync(user.Id, model.Role.ToString());
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "An error occurred while creating the user.");
            }

            return View(model);
        }
    }
}
