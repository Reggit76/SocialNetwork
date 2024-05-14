using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Models.Entity;
using SocialNetwork.Data;

public class FriendshipService
{
    private readonly UserDbContext _context;

    public FriendshipService(UserDbContext context)
    {
        _context = context;
    }

    public bool AddFriend(int userId, int friendId)
    {
        if (_context.Friendships.Any(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId)))
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

    public List<int> GetFriends(int userId)
    {
        return _context.Friendships
            .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == FriendshipStatus.Accepted)
            .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
            .ToList();
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
}
