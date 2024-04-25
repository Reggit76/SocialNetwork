using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SocialNetwork.Models
{
    public class User
    {
        public int Id { get; set; }

        [MinLength(DataConstants.NameMinLength), MaxLength(DataConstants.NameMaxLength)]
        public string Name { get; set; }

        [MinLength(DataConstants.NameMinLength), MaxLength(DataConstants.NameMaxLength)]
        public string LastName {  get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }

        // Отношения
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

    }
}
