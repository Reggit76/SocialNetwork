using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.DTO;
using SocialNetwork.Services.Interfaces;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAllPostsAsync();
            return View(posts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostDTO post)
        {
            if (ModelState.IsValid)
            {
                await _postService.CreatePostAsync(post);
                return RedirectToAction("Index");
            }
            return View(post);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostDTO post)
        {
            if (ModelState.IsValid)
            {
                await _postService.UpdatePostAsync(post);
                return RedirectToAction("Index");
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _postService.DeletePostAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, CommentDTO comment)
        {
            if (ModelState.IsValid)
            {
                await _postService.AddCommentAsync(comment);
                return RedirectToAction("Details", new { id = postId });
            }
            return View(comment);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            await _postService.DeleteCommentAsync(commentId);
            return RedirectToAction("Index");
        }
    }
}
