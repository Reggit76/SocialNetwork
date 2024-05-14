using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SocialNetwork.Models.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public User()
        {
            Username = string.Empty;
            Email = string.Empty;
        }

        public User(string username, string email)
        {
            Username = username;
            Email = email;
        }
    }
}
