namespace SocialNetwork.Models
{
    public enum FriendshipStatus
    {
        None,        // Нет отношений дружбы
        Pending,     // Запрос отправлен, но еще не принят
        Accepted,    // Запрос принят, дружба установлена
        Declined     // Запрос отклонен
    }
}
