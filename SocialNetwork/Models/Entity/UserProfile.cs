namespace SocialNetwork.Models.Entity
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Location { get; set; }
        public string ProfilePictureUrl { get; set; }
        // Отношения
        public virtual ICollection<Post> Posts { get; set; }

        public UserProfile()
        {
        }

        public UserProfile(string fullName, string bio, DateTime dateOfBirth, string location)
        {
            FullName = fullName;
            Bio = bio;
            DateOfBirth = dateOfBirth;
            Location = location;
        }
    }
}
