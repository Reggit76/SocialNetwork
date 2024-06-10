using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<bool> SendFriendRequestAsync(int userId, int friendId)
        {
            var user = await _context.Users.FindAsync(userId);
            var friend = await _context.Users.FindAsync(friendId);

            if (user == null || friend == null)
            {
                throw new InvalidOperationException("User or Friend not found");
            }

            if (await _context.Friendships.AnyAsync(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId)))
            {
                return false; 
            }

            var friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId,
                Status = FriendshipStatus.Pending,
                RequestDate = DateTime.UtcNow
            };

            await _context.Friendships.AddAsync(friendship);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> AcceptFriendRequestAsync(int userId, int friendId)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);
            if (friendship != null)
            {
                friendship.Status = FriendshipStatus.Accepted;
                friendship.AcceptanceDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeclineFriendRequestAsync(int userId, int friendId)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);
            if (friendship != null)
            {
                friendship.Status = FriendshipStatus.Declined;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveFriendAsync(int userId, int friendId)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId));
            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<UserDTO>> GetFriendsAsync(int userId)
        {
            var friendships = await _context.Friendships
                .Where(f => (f.UserId == userId && f.Status == FriendshipStatus.Accepted)
                            || (f.FriendId == userId && f.Status == FriendshipStatus.Accepted))
                .ToListAsync();

            var friendIds = friendships.Select(f => f.UserId == userId ? f.FriendId : f.UserId).Distinct().ToList();

            return await _context.Users
                .Where(u => friendIds.Contains(u.Id))
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


        public async Task<List<UserDTO>> GetPendingRequestsAsync(int userId)
        {
            return await _context.Friendships
                .Where(f => f.FriendId == userId && f.Status == FriendshipStatus.Pending)
                .Select(f => new UserDTO
                {
                    Id = f.User.Id,
                    FullName = f.User.FullName,
                    Email = f.User.Email,
                    Gender = f.User.Gender,
                    DateOfBirth = f.User.DateOfBirth,
                    ProfilePictureUrl = f.User.ProfilePictureUrl,
                    Role = f.User.Role,
                    Username = f.User.Username,
                    IsBanned = f.User.IsBanned
                })
                .ToListAsync();
        }

        public async Task<FriendshipStatus> GetFriendshipStatusAsync(int userId, int friendId)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId));
            return friendship?.Status ?? FriendshipStatus.None;
        }
    }
}
