using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.DTO;
using SocialNetwork.Services;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        public IActionResult Index()
        {
            var posts = _postService.GetAllPosts();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PostDTO postDTO)
        {
            if (ModelState.IsValid)
            {
                _postService.CreatePost(postDTO);
                return RedirectToAction("Index");
            }
            return View(postDTO);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var post = _postService.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public IActionResult Edit(PostDTO postDTO)
        {
            if (ModelState.IsValid)
            {
                _postService.UpdatePost(postDTO);
                return RedirectToAction("Index");
            }
            return View(postDTO);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var post = _postService.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _postService.DeletePost(id);
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var post = _postService.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public IActionResult AddComment(CommentDTO commentDTO)
        {
            if (ModelState.IsValid)
            {
                _postService.AddComment(commentDTO);
                return RedirectToAction("Details", new { id = commentDTO.PostId });
            }
            return RedirectToAction("Details", new { id = commentDTO.PostId });
        }

        [HttpPost]
        public IActionResult DeleteComment(int commentId, int postId)
        {
            _postService.DeleteComment(commentId);
            return RedirectToAction("Details", new { id = postId });
        }
    }
}
