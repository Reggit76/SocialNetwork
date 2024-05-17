using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SocialNetwork.Models.Entity;
using SocialNetwork.Data;
using SocialNetwork.Models;

namespace SocialNetwork.Services
{
    public class UserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
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

            // Создание профиля пользователя
            var userProfile = new UserProfile
            {
                UserId = user.Id,
                Role = UserRole.RegularUser // Значение по умолчанию
            };
            _context.UserProfiles.Add(userProfile);
            _context.SaveChanges();

            // Сохранить хэш пароля отдельно в таблице паролей
            var passwordHash = HashPassword(password);
            SavePasswordHash(user.Id, passwordHash);

            return true;
        }

        public User AuthenticateUser(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                var storedPasswordHash = GetPasswordHash(user.Id);
                if (VerifyPassword(password, storedPasswordHash))
                {
                    return user;
                }
            }

            return null;
        }

        public UserProfile GetUserProfile(int userId)
        {
            return _context.UserProfiles.FirstOrDefault(p => p.UserId == userId);
        }

        public bool UpdateUserProfile(UserProfile profile)
        {
            var existingProfile = _context.UserProfiles.FirstOrDefault(p => p.UserId == profile.UserId);
            if (existingProfile != null)
            {
                existingProfile.FullName = profile.FullName;
                existingProfile.Bio = profile.Bio;
                existingProfile.DateOfBirth = profile.DateOfBirth;
                existingProfile.ProfilePictureUrl = profile.ProfilePictureUrl;
                existingProfile.Role = profile.Role;

                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public List<Chat> GetUserChats(int userId)
        {
            return _context.ChatUsers
                .Where(cu => cu.UserId == userId)
                .Include(cu => cu.Chat)
                .Select(cu => cu.Chat)
                .ToList();
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
            using (var connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO UserPasswords (UserId, PasswordHash) VALUES (@userId, @passwordHash)", connection))
                {
                    command.Parameters.AddWithValue("userId", userId);
                    command.Parameters.AddWithValue("passwordHash", passwordHash);
                    command.ExecuteNonQuery();
                }
            }
        }

        private string GetPasswordHash(int userId)
        {
            using (var connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT PasswordHash FROM UserPasswords WHERE UserId = @userId", connection))
                {
                    command.Parameters.AddWithValue("userId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }
            return null;
        }
    }
}