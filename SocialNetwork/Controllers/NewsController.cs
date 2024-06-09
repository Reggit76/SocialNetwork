using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Services.Interfaces;
using SocialNetwork.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using SocialNetwork.Models.DTO;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;

        public NewsController(IPostService postService, IUserService userService, ICommentService commentService)
        {
            _postService = postService;
            _userService = userService;
            _commentService = commentService;
        }

        public async Task<IActionResult> Index(string filter = "all")
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);

            var posts = filter switch
            {
                "friends" => await _postService.GetFriendsPostsAsync(userId),
                _ => await _postService.GetAllPostsAsync()
            };

            var model = new NewsViewModel
            {
                Posts = posts.ToList(),
                Filter = filter
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = await _userService.GetUserIdFromClaimsAsync(User);
                var postDto = new PostDTO
                {
                    UserId = userId,
                    Content = model.Content,
                    DatePosted = DateTime.UtcNow,
                    ImageUrl = model.ImageUrl
                };

                await _postService.CreatePostAsync(postDto);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            if (post.UserId != userId && !User.IsInRole("Administrator") && !User.IsInRole("Moderator"))
            {
                return Forbid();
            }

            var model = new EditPostViewModel
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = await _userService.GetUserIdFromClaimsAsync(User);
                var post = await _postService.GetPostByIdAsync(model.Id);

                if (post == null)
                {
                    return NotFound();
                }

                if (post.UserId != userId && !User.IsInRole("Administrator") && !User.IsInRole("Moderator"))
                {
                    return Forbid();
                }

                post.Content = model.Content;
                post.ImageUrl = model.ImageUrl;

                await _postService.UpdatePostAsync(post);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            if (post.UserId != userId && !User.IsInRole("Administrator") && !User.IsInRole("Moderator"))
            {
                return Forbid();
            }

            await _postService.DeletePostAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> LikePost(int postId)
        {
            var result = await _postService.LikePostAsync(postId);
            if (result)
            {
                return RedirectToAction("Index");
            }
            return BadRequest("Unable to like post");
        }

        [HttpPost]
        public async Task<IActionResult> DislikePost(int postId)
        {
            var result = await _postService.DislikePostAsync(postId);
            if (result)
            {
                return RedirectToAction("Index");
            }
            return BadRequest("Unable to dislike post");
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Comment content cannot be empty");
            }

            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            var commentDTO = new CommentDTO
            {
                PostId = postId,
                UserId = userId,
                Content = content,
                DatePosted = DateTime.UtcNow,
                UserFullName = User.Identity.Name,
                UserProfilePictureUrl = User.FindFirst("ProfilePictureUrl")?.Value
            };

            await _commentService.AddCommentAsync(commentDTO);
            return RedirectToAction("Index");
        }
    }
}
