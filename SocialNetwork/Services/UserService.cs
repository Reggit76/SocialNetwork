using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Models.DTO;
using SocialNetwork.Data;
using SocialNetwork.Models;
using SocialNetwork.Services.Interfaces;
using SocialNetwork.Models.ViewModels;

namespace SocialNetwork.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetUserIdAsync(string Username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == Username);
            return user?.Id ?? 0;
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

        public bool RegisterUserAsAdmin(string username, string email, string password, Role role, string fullName, Gender gender, DateTime dateOfBirth, string? profilePictureUrl, string? description)
        {
            if (_context.Users.Any(u => u.Username == username || u.Email == email))
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
                FullName = fullName,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                ProfilePictureUrl = profilePictureUrl,
                Description = description,
                Role = role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

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
                        UserId = u.Id,
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
                    UserId = u.Id,
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

        public async Task<UserDTO> AuthenticateUserAsync(string Username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == Username);
            if (user != null)
            {
                var storedPasswordHash = GetPasswordHash(user.Id);
                if (VerifyPassword(password, storedPasswordHash))
                {
                    return new UserDTO
                    {
                        UserId = user.Id,
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

        public async Task<UserDTO> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            return new UserDTO
            {
                UserId = user.Id,
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

        public async Task<bool> UpdateUserProfileAsync(UserDTO User)
        {
            var user = await _context.Users.FindAsync(User.UserId);
            if (user == null)
            {
                return false;
            }

            user.FullName = User.FullName;
            user.Gender = User.Gender;
            user.DateOfBirth = User.DateOfBirth;
            user.ProfilePictureUrl = User.ProfilePictureUrl;
            user.Email = User.Email;
            user.Username = User.Username;
            user.IsBanned = User.IsBanned;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users.Select(u => new UserDTO
            {
                UserId = u.Id,
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
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == passwordHash;
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
    }
}
