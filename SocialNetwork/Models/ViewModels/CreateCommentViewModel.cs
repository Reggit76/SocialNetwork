namespace SocialNetwork.Models.ViewModels
{
    public class CreateCommentViewModel
    {
        public int PostId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
