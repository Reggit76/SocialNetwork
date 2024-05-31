using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Models.DTO;
using SocialNetwork.Data;
using SocialNetwork.Models;
using SocialNetwork.Services.Interfaces;

namespace SocialNetwork.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly UserDbContext _context;

        public FriendshipService(UserDbContext context)
        {
            _context = context;
        }

        public bool SendFriendRequest(int userId, int friendId)
        {
            if (_context.Friendships.Any(f => f.UserId == userId && f.FriendId == friendId ||
                                              f.UserId == friendId && f.FriendId == userId))
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

        public bool RemoveFriend(int userId, int friendId)
        {
            var friendship = _context.Friendships.FirstOrDefault(f => f.UserId == userId && f.FriendId == friendId ||
                                                                      f.UserId == friendId && f.FriendId == userId);
            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<UserProfileDTO> GetFriends(int userId)
        {
            return _context.Friendships
                .Where(f => f.UserId == userId && f.Status == FriendshipStatus.Accepted)
                .Include(f => f.Friend)
                .Select(f => new UserProfileDTO
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

        public List<UserProfileDTO> GetIncomingFriendRequests(int userId)
        {
            return _context.Friendships
                .Where(f => f.FriendId == userId && f.Status == FriendshipStatus.Pending)
                .Include(f => f.User)
                .Select(f => new UserProfileDTO
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

        public bool IsFriend(int userId, int friendId)
        {
            return _context.Friendships.Any(f => f.UserId == userId && f.FriendId == friendId && f.Status == FriendshipStatus.Accepted ||
                                                 f.UserId == friendId && f.FriendId == userId && f.Status == FriendshipStatus.Accepted);
        }

        public bool HasPendingRequest(int userId, int friendId)
        {
            return _context.Friendships.Any(f => f.UserId == friendId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);
        }

        public List<FriendshipDTO> GetPendingRequests(int userId)
        {
            return _context.Friendships
                .Where(f => f.FriendId == userId && f.Status == FriendshipStatus.Pending)
                .Select(f => new FriendshipDTO
                {
                    UserId = f.UserId,
                    FriendId = f.FriendId,
                    Status = f.Status,
                    RequestDate = f.RequestDate,
                    AcceptanceDate = f.AcceptanceDate
                })
                .ToList();
        }
    }
}
