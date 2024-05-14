using SocialNetwork.Models.Entity;

namespace SocialNetwork.Models.Service
{
    public class NewsFeed
    {
        public List<Post> Posts { get; set; } = new List<Post>();

        // Метод для добавления поста в ленту
        public void AddPost(Post post)
        {
            Posts.Add(post);
            // Тут можно добавить логику сортировки или фильтрации
        }

        // Метод для получения постов для отображения пользователю
        public List<Post> GetPostsForUser(UserProfile user)
        {
            // Пример простой фильтрации: возвращаем посты, созданные друзьями пользователя
            return Posts.Where(p => user.Friends.Contains(p.UserId)).ToList();
        }
    }

}
