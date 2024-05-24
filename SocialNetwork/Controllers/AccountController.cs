using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using SocialNetwork.Models.AccountViewModels;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.Entity;
using SocialNetwork.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_userService.RegisterUser(model.Username, model.Email, model.Password))
                {
                    var user = _userService.AuthenticateUser(model.Username, model.Password);
                    if (user != null)
                    {
                        await AuthenticateUserAsync(user);
                        TempData["UserId"] = user.Id;
                        return RedirectToAction("CompleteProfile");
                    }
                }
                ModelState.AddModelError("", "Registration failed");
            }
            return View(model);
        }

        private async Task AuthenticateUserAsync(UserDTO user)
        {
            var role = _userService.GetUserRole(user.Id);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.AuthenticateUser(model.Username, model.Password);
                if (user != null)
                {
                    await AuthenticateUserAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Login failed");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult RegisterAdmin(RegisterAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_userService.RegisterUserAsAdmin(model.Username, model.Email, model.Password, model.Role))
                {
                    return RedirectToAction("ManageUsers");
                }
                ModelState.AddModelError("", "Registration failed");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult ManageUsers()
        {
            var users = _userService.GetAllUsers();
            return View(users);
        }

        [HttpGet]
        [Authorize]
        public IActionResult CompleteProfile()
        {
            if (TempData["UserId"] is int userId)
            {
                return View(new UserProfileViewModel { UserId = userId });
            }

            // Если UserId не найден в TempData, отобразим сообщение об ошибке
            ModelState.AddModelError("", "Unable to complete profile. Please try again.");
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Authorize]
        public IActionResult CompleteProfile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userProfile = new UserProfileDTO
                {
                    UserId = model.UserId,
                    FullName = model.FullName,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    ProfilePictureUrl = model.ProfilePictureUrl
                };
                if (_userService.UpdateUserProfile(userProfile))
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Failed to complete profile");
            }
            return View(model);
        }
    }
}
