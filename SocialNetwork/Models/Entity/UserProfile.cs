namespace SocialNetwork.Models.Entity
{
    public enum UserRole
    {
        RegularUser, // Обычный пользователь
        Moderator, // Модератор
        Administrator // Администратор
    }

    public enum Gender
    {
        Male, // Мужчина
        Female, // Женщина
        Hidden // Скрыт 
    }

    public class UserProfile
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public Gender Bio { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ProfilePictureUrl { get; set; }
        public UserRole Role { get; set; } // Добавлено поле роли

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
