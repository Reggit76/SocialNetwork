namespace SocialNetwork.Models.Entity
{
        public class UserProfile
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public Gender Bio { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ProfilePictureUrl { get; set; }
        public UserRole Role { get; set; }

        public List<ChatUser> Chats { get; set; } = new List<ChatUser>(); // Многие ко многим 

        public UserProfile()
        {
            FullName = string.Empty;
            Bio = Gender.Hidden;
            Role = UserRole.RegularUser; // Установить обычного пользователя по умолчанию
            ProfilePictureUrl = "../../images/Avatar/default.jpg";
        }

        public UserProfile(string fullName, Gender bio, DateTime dateOfBirth, UserRole role = UserRole.RegularUser)
        {
            FullName = fullName;
            Bio = bio;
            DateOfBirth = dateOfBirth;
            Role = role;
            ProfilePictureUrl = "../../images/Avatar/default.jpg";
        }
    }
}
