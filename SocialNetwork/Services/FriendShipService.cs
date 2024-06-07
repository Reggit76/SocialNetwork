using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Models;
using SocialNetwork.Models.DTO;
using SocialNetwork.Models.Entity;
using SocialNetwork.Services.Interfaces;

namespace SocialNetwork.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly ApplicationDbContext _context;

        public FriendshipService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool SendFriendRequest(int userId, int friendId)
        {
            if (_context.Friendships.Any(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId)))
            {
                return false; // Дружба уже существует или запрос уже отправлен
            }

            var friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId,
                Status = FriendshipStatus.Pending,
                RequestDate = DateTime.UtcNow
            };

            _context.Friendships.Add(friendship);
            _context.SaveChanges();
            return true;
        }

        public bool AcceptFriendRequest(int userId, int friendId)
        {
            var friendship = _context.Friendships.FirstOrDefault(f => f.UserId == friendId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);
            if (friendship != null)
            {
                friendship.Status = FriendshipStatus.Accepted;
                friendship.AcceptanceDate = DateTime.UtcNow;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeclineFriendRequest(int userId, int friendId)
        {
            var friendship = _context.Friendships.FirstOrDefault(f => f.UserId == friendId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);
            if (friendship != null)
            {
                friendship.Status = FriendshipStatus.Declined;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RemoveFriend(int userId, int friendId)
        {
            var friendship = _context.Friendships.FirstOrDefault(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId));
            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<UserDTO> GetFriends(int userId)
        {
            return _context.Friendships
                .Where(f => f.UserId == userId && f.Status == FriendshipStatus.Accepted)
                .Select(f => new UserDTO
                {
                    UserId = f.Friend.UserId,
                    FullName = f.Friend.FullName,
                    Gender = f.Friend.Gender,
                    DateOfBirth = f.Friend.DateOfBirth,
                    ProfilePictureUrl = f.Friend.ProfilePictureUrl,
                    Role = f.Friend.Role
                })
                .ToList();
        }

        public List<UserDTO> GetPendingRequests(int userId)
        {
            return _context.Friendships
                .Where(f => f.FriendId == userId && f.Status == FriendshipStatus.Pending)
                .Select(f => new UserDTO
                {
                    UserId = f.User.UserId,
                    FullName = f.User.FullName,
                    Gender = f.User.Gender,
                    DateOfBirth = f.User.DateOfBirth,
                    ProfilePictureUrl = f.User.ProfilePictureUrl,
                    Role = f.User.Role
                })
                .ToList();
        }

        public FriendshipStatus GetFriendshipStatus(int userId, int friendId)
        {
            var friendship = _context.Friendships.FirstOrDefault(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId));
            return friendship?.Status ?? FriendshipStatus.None;
        }
    }
}
