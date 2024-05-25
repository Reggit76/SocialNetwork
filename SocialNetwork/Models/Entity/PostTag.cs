namespace SocialNetwork.Models.Entity
{
    public class PostTag
    {
        public int PostId { get; set; }
        public virtual Post? Post { get; set; }

        public string Tag { get; set; } = null!;
    }
}
