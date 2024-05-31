using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Models.DTO;
using SocialNetwork.Data;
using SocialNetwork.Models;
using SocialNetwork.Services.Interfaces;

namespace SocialNetwork.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
        }

        public int GetUserId(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            return user.Id;
        }

        public bool RegisterUser(string username, string email, string password)
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
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Создание профиля пользователя с начальным значением FullName
            var userProfile = new UserProfile
            {
                UserId = user.Id,
                FullName = "", // Инициализация пустым значением, чтобы удовлетворить ограничение NOT NULL
                Role = UserRole.RegularUser
            };
            _context.UserProfiles.Add(userProfile);
            _context.SaveChanges();

            // Сохранить хэш пароля отдельно в таблице Users
            var passwordHash = HashPassword(password);
            SavePasswordHash(user.Id, passwordHash);

            return true;
        }

        public bool RegisterUserAsAdmin(string username, string email, string password, UserRole role)
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
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Создание профиля пользователя с указанной ролью и начальным значением FullName
            var userProfile = new UserProfile
            {
                UserId = user.Id,
                FullName = "", // Инициализация пустым значением, чтобы удовлетворить ограничение NOT NULL
                Role = role
            };
            _context.UserProfiles.Add(userProfile);
            _context.SaveChanges();

            // Сохранить хэш пароля отдельно в таблице Users
            var passwordHash = HashPassword(password);
            SavePasswordHash(user.Id, passwordHash);

            return true;
        }

        public List<UserProfileDTO> SearchUsers(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return _context.UserProfiles
                    .Select(u => new UserProfileDTO
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        Gender = u.Gender,
                        DateOfBirth = u.DateOfBirth,
                        ProfilePictureUrl = u.ProfilePictureUrl,
                        Role = u.Role
                    }).ToList();
            }
            return _context.UserProfiles
                .Where(u => u.FullName.Contains(searchTerm) || u.User.Username.Contains(searchTerm))
                .Select(u => new UserProfileDTO
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Gender = u.Gender,
                    DateOfBirth = u.DateOfBirth,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    Role = u.Role
                }).ToList();
        }

        public UserDTO AuthenticateUser(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                var storedPasswordHash = GetPasswordHash(user.Id);
                if (VerifyPassword(password, storedPasswordHash))
                {
                    return new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email
                    };
                }
            }

            return null;
        }

        public UserProfileDTO GetUserProfile(int userId)
        {
            var profile = _context.UserProfiles.FirstOrDefault(p => p.UserId == userId);
            if (profile != null)
            {
                return new UserProfileDTO
                {
                    UserId = profile.UserId,
                    FullName = profile.FullName,
                    Gender = profile.Gender,
                    DateOfBirth = profile.DateOfBirth,
                    ProfilePictureUrl = profile.ProfilePictureUrl,
                    Role = profile.Role
                };
            }
            return null;
        }

        public bool UpdateUserProfile(UserProfileDTO profile)
        {
            var existingProfile = _context.UserProfiles.FirstOrDefault(p => p.UserId == profile.UserId);
            if (existingProfile != null)
            {
                existingProfile.FullName = profile.FullName;
                existingProfile.Gender = profile.Gender;
                existingProfile.DateOfBirth = profile.DateOfBirth;
                existingProfile.ProfilePictureUrl = profile.ProfilePictureUrl;
                existingProfile.Role = profile.Role;

                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public List<ChatDTO> GetUserChats(int userId)
        {
            return _context.ChatUsers
                .Where(cu => cu.UserId == userId)
                .Include(cu => cu.Chat)
                .ThenInclude(c => c.Messages)
                .Include(cu => cu.Chat)
                .ThenInclude(c => c.Participants)
                .ThenInclude(cp => cp.UserProfile)
                .Select(cu => new ChatDTO
                {
                    Id = cu.Chat.Id,
                    Name = cu.Chat.Name,
                    Description = cu.Chat.Description,
                    Participants = cu.Chat.Participants.Select(p => new UserProfileDTO
                    {
                        UserId = p.UserProfile.UserId,
                        FullName = p.UserProfile.FullName,
                        Gender = p.UserProfile.Gender,
                        DateOfBirth = p.UserProfile.DateOfBirth,
                        ProfilePictureUrl = p.UserProfile.ProfilePictureUrl,
                        Role = p.UserProfile.Role
                    }).ToList(),
                    Messages = cu.Chat.Messages.Select(m => new MessageDTO
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        ChatId = m.ChatId,
                        Content = m.Content,
                        Timestamp = m.Timestamp
                    }).ToList()
                }).ToList();
        }

        public UserRole GetUserRole(int userId)
        {
            var profile = _context.UserProfiles.FirstOrDefault(p => p.UserId == userId);
            return profile?.Role ?? UserRole.RegularUser;
        }

        public void UpdateUserRole(int userId, UserRole newRole)
        {
            var profile = _context.UserProfiles.FirstOrDefault(p => p.UserId == userId);
            if (profile != null)
            {
                profile.Role = newRole;
                _context.SaveChanges();
            }
        }

        public List<UserDTO> GetAllUsers()
        {
            return _context.Users
                .Include(u => u.UserProfile)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                }).ToList();
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