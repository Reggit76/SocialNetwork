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
using Microsoft.Extensions.Logging;

namespace SocialNetwork.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> GetUserIdAsync(string username)
        {
            _logger.LogInformation("Retrieving user ID for username {username}.", username);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            return user?.Id ?? 0;
        }

        public async Task<int> GetUserIdFromClaimsAsync(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogInformation("User ID {userId} retrieved from claims.", userId);
                return userId;
            }
            _logger.LogWarning("User ID claim not found or invalid.");
            return 0;
        }

        public async Task<bool> RegisterUserAsync(RegisterViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email))
            {
                _logger.LogWarning("Username or email already exists for username {username}, email {email}.", model.Username, model.Email);
                return false;
            }

            if (!IsValidPassword(model.Password))
            {
                _logger.LogWarning("Invalid password for username {username}.", model.Username);
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

            _logger.LogInformation("User {username} registered successfully.", model.Username);
            return true;
        }

        public async Task<bool> RegisterUserAsAdminAsync(string username, string email, string password, Role role)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email))
            {
                _logger.LogWarning("Username or email already exists for username {username}, email {email}.", username, email);
                return false;
            }

            if (!IsValidPassword(password))
            {
                _logger.LogWarning("Invalid password for username {username}.", username);
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

            _logger.LogInformation("Admin user {username} registered successfully.", username);
            return true;
        }

        public async Task<List<UserDTO>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.LogInformation("Retrieving all users.");
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

            _logger.LogInformation("Searching users with term {searchTerm}.", searchTerm);
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
            _logger.LogInformation("Authenticating user with email {email}.", email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                var storedPasswordHash = GetPasswordHash(user.Id);
                if (VerifyPassword(password, storedPasswordHash))
                {
                    _logger.LogInformation("User with email {email} authenticated successfully.", email);
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
            _logger.LogWarning("Authentication failed for email {email}.", email);
            return null;
        }

        public async Task<UserDTO> GetUserProfileAsync(int id)
        {
            _logger.LogInformation("Retrieving profile for user ID {userId}.", id);
            var user = await _context.Users
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {userId} not found.", id);
                return null;
            }

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

            _logger.LogInformation("Profile for user ID {userId} retrieved successfully.", id);
            return userDto;
        }

        public async Task<IEnumerable<PostDTO>> GetUserPostsAsync(int userId)
        {
            _logger.LogInformation("Retrieving posts for user ID {userId}.", userId);
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
            _logger.LogInformation("Updating profile for user ID {userId}.", userDto.Id);
            var user = await _context.Users.FindAsync(userDto.Id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {userId} not found.", userDto.Id);
                return false;
            }

            if (await _context.Users.AnyAsync(u => (u.Email == userDto.Email || u.Username == userDto.Username) && u.Id != userDto.Id))
            {
                _logger.LogWarning("Email or username already exists for another user (ID {userId}).", userDto.Id);
                return false; 
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

            _logger.LogInformation("Profile for user ID {userId} updated successfully.", userDto.Id);
            return true;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            _logger.LogInformation("Retrieving all users.");
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
            _logger.LogInformation("Creating user {username}.", User.Username);
            if (await _context.Users.AnyAsync(u => u.Username == User.Username || u.Email == User.Email))
            {
                _logger.LogWarning("Username or email already exists for username {username}, email {email}.", User.Username, User.Email);
                return false;
            }

            if (!IsValidPassword(password))
            {
                _logger.LogWarning("Invalid password for username {username}.", User.Username);
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

            _logger.LogInformation("User {username} created successfully.", User.Username);
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
            _logger.LogInformation("Banning user with ID {userId}.", userId);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {userId} not found.", userId);
                return false;
            }

            user.IsBanned = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User with ID {userId} banned.", userId);
            return true;
        }

        public async Task<bool> UnbanUserAsync(int userId)
        {
            _logger.LogInformation("Unbanning user with ID {userId}.", userId);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {userId} not found.", userId);
                return false;
            }

            user.IsBanned = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User with ID {userId} unbanned.", userId);
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
            _logger.LogInformation("Retrieving role for username {username}.", username);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                return user.Role.ToString();
            }
            _logger.LogWarning("User with username {username} not found.", username);
            return null;
        }

        public async Task<List<UserDTO>> GetAllUsersExceptFriendsAsync(int userId, List<int> friendIds)
        {
            _logger.LogInformation("Retrieving all users except friends for user ID {userId}.", userId);
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
                    IsBanned = u.IsBanned
                }).ToListAsync();
        }
    }
}
