using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName {  get; set; }
        [Required]
        public string Email { get; set; }
        public DateTime Birthday { get; set; }

    }
}
