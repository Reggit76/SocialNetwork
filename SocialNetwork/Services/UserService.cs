using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Models.DTO;
using SocialNetwork.Data;
using SocialNetwork.Services.Interfaces;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Models;

namespace SocialNetwork.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetUserIdAsync(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            return user?.Id ?? 0;
        }

        public async Task<int> GetUserIdFromClaimsAsync(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }

        public async Task<bool> RegisterUserAsync(RegisterViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email))
            {
                return false;
            }

            if (!IsValidPassword(model.Password))
            {
                throw new ArgumentException("Password does not meet the required criteria.");
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                ProfilePictureUrl = model.ProfilePictureUrl,
                Role = model.Role
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var passwordHash = HashPassword(model.Password);
            SavePasswordHash(user.Id, passwordHash);

            return true;
        }

        public async Task<bool> RegisterUserAsAdminAsync(string username, string email, string password, Role role)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email))
            {
                return false;
            }

            if (!IsValidPassword(password))
            {
                throw new ArgumentException("Password does not meet the required criteria.");
            }

            var user = new User
            {
                Username = username,
                Email = email,
                Role = role
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var passwordHash = HashPassword(password);
            SavePasswordHash(user.Id, passwordHash);

            return true;
        }

        public async Task<List<UserDTO>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _context.Users
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        Gender = u.Gender,
                        DateOfBirth = u.DateOfBirth,
                        ProfilePictureUrl = u.ProfilePictureUrl,
                        Role = u.Role,
                        Username = u.Username,
                        IsBanned = u.IsBanned
                    }).ToListAsync();
            }
            return await _context.Users
                .Where(u => u.FullName.Contains(searchTerm) || u.Username.Contains(searchTerm))
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Gender = u.Gender,
                    DateOfBirth = u.DateOfBirth,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    Role = u.Role,
                    Username = u.Username,
                    IsBanned = u.IsBanned
                }).ToListAsync();
        }

        public async Task<UserDTO> AuthenticateUserAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                var storedPasswordHash = GetPasswordHash(user.Id);
                if (VerifyPassword(password, storedPasswordHash))
                {
                    return new UserDTO
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Gender = user.Gender,
                        DateOfBirth = user.DateOfBirth,
                        ProfilePictureUrl = user.ProfilePictureUrl,
                        Role = user.Role,
                        Username = user.Username,
                        IsBanned = user.IsBanned
                    };
                }
            }

            return null;
        }

        public async Task<UserDTO> GetUserProfileAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            var userDto = new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Role = user.Role,
                Username = user.Username,
                IsBanned = user.IsBanned,
                Posts = user.Posts.Select(p => new PostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    DatePosted = p.DatePosted
                }).ToList()
            };

            return userDto;
        }

        public async Task<IEnumerable<PostDTO>> GetUserPostsAsync(int userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    LikesCount = p.LikesCount,
                    ImageUrl = p.ImageUrl
                }).ToListAsync();
        }

        public async Task<bool> UpdateUserProfileAsync(UserDTO userDto)
        {
            var user = await _context.Users.FindAsync(userDto.Id);
            if (user == null)
            {
                return false;
            }

            // Проверка на существование электронной почты и имени пользователя, исключая текущего пользователя
            if (await _context.Users.AnyAsync(u => (u.Email == userDto.Email || u.Username == userDto.Username) && u.Id != userDto.Id))
            {
                return false; // Email или Username уже существуют для другого пользователя
            }

            user.FullName = userDto.FullName;
            user.Gender = userDto.Gender;
            user.DateOfBirth = userDto.DateOfBirth;
            user.ProfilePictureUrl = userDto.ProfilePictureUrl;
            user.Email = userDto.Email;
            user.Username = userDto.Username;
            user.IsBanned = userDto.IsBanned;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users.Select(u => new UserDTO
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Gender = u.Gender,
                DateOfBirth = u.DateOfBirth,
                ProfilePictureUrl = u.ProfilePictureUrl,
                Role = u.Role,
                Username = u.Username,
                IsBanned = u.IsBanned
            }).ToListAsync();
        }

        public async Task<bool> CreateUserAsync(UserDTO User, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Username == User.Username || u.Email == User.Email))
            {
                return false;
            }

            if (!IsValidPassword(password))
            {
                throw new ArgumentException("Password does not meet the required criteria.");
            }

            var user = new User
            {
                Username = User.Username,
                Email = User.Email,
                FullName = User.FullName,
                Gender = User.Gender,
                DateOfBirth = User.DateOfBirth,
                ProfilePictureUrl = User.ProfilePictureUrl,
                Role = User.Role
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var passwordHash = HashPassword(password);
            SavePasswordHash(user.Id, passwordHash);

            return true;
        }

        public async Task<bool> ChangeUserRoleAsync(int userId, string role)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !Enum.TryParse(role, out Role newRole))
            {
                return false;
            }

            user.Role = newRole;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> BanUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsBanned = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnbanUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsBanned = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, Role newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Role = newRole;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        private static bool IsValidPassword(string password)
        {
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        private void SavePasswordHash(int userId, string passwordHash)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                _context.Entry(user).Property("PasswordHash").CurrentValue = passwordHash;
                _context.SaveChanges();
            }
        }

        private string GetPasswordHash(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                return _context.Entry(user).Property("PasswordHash").CurrentValue as string;
            }
            return null;
        }

        public async Task<string> GetUserRoleAsync(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                return user.Role.ToString();
            }
            return null;
        }

        public async Task<List<UserDTO>> GetAllUsersExceptFriendsAsync(int userId, List<int> friendIds)
        {
            return await _context.Users
                .Where(u => u.Id != userId && !friendIds.Contains(u.Id))
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Gender = u.Gender,
                    DateOfBirth = u.DateOfBirth,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    Role = u.Role,
                    Username = u.Username,
                    IsBanned = u.IsBanned,
                    Description = u.Description
                }).ToListAsync();
        }
    }
}
