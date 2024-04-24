using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class UserSession
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}
