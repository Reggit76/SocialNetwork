using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SocialNetwork.Models.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserProfile Profile { get; set; }

        public User()
        {
        }

        public User(string username, string email, string passwordHash)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
