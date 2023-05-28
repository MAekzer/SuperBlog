using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Data.Repositories;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;

namespace SuperBlog.Controllers
{
    public class CommentController : Controller
    {
        private readonly IRepository<Comment> commentRepo;
        private readonly IRepository<Post> postRepo;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public CommentController(IRepository<Comment> commentRepository, IRepository<Post> postRepository, IMapper mapper, UserManager<User> userManager)
        {
            commentRepo = commentRepository;
            postRepo = postRepository;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        [Route("PostComments")]
        public async Task<IActionResult> GetComments(string postId)
        {
            var post = await postRepo.GetByIdAsync(postId);
            if (post == null) return View("/Views/Posts/PostNotFound.cshtml");

            var comments = commentRepo.GetAll().Where(c => c.PostId.Equals(postId)).ToList();

            var model = new CommentsViewModel { Post = post, Comments = comments };
            return View("/Views/Comments/CommentSection.cshtml", model);
        }

        [HttpGet]
        [Route("Comment")]
        public async Task<IActionResult> GetComment(string id)
        {
            var comment = await postRepo.GetByIdAsync(id);
            if (comment == null) return View("/Views/Comments/CommentNotFound.cshtml");

            return View("/Views/Comments/Comment.cshtml", comment);
        }

        [HttpGet]
        [Route("EditComment")]
        public async Task<IActionResult> Edit(string id)
        {
            var comment = await commentRepo.GetByIdAsync(id);
            if (comment == null) return View("/Views/Comments/CommentNotFound.cshtml");

            var model = new WriteCommentViewModel { Content = comment.Content };

            return View("/Views/Comments/WriteComment.cshtml", model);
        }

        [HttpPost]
        [Route("EditComment")]
        public async Task<IActionResult> Edit(WriteCommentViewModel model, string commentId, string postId)
        {
            var comment = await commentRepo.GetByIdAsync(commentId);
            if (comment == null) return View("/Views/Comments/CommentNotFound.cshtml");

            comment.IsRedacted = true;
            comment.RedactionTime = DateTime.Now;
            comment.Content = model.Content;

            await commentRepo.UpdateAsync(comment);
            var post = await postRepo.GetByIdAsync(postId);

            return View("/Views/Posts/Post.cshtml", post);
        }

        [HttpGet]
        [Route("NewComment")]
        public async Task<IActionResult> Write(string postId)
        {
            return View("/Views/Comments/WriteComment.cshtml");
        }

        [HttpPost]
        [Route("NewComment")]
        public async Task<IActionResult> Write(WriteCommentViewModel model, string postId)
        {
            var post = await postRepo.GetByIdAsync(postId);
            var user = await userManager.GetUserAsync(User);
            if (post == null || user == null) return RedirectToAction("Index", "Home");

            var comment = new Comment
            {
                Content = model.Content,
                Post = post,
                User = user
            };

            await commentRepo.AddAsync(comment);
            return View("/Views/Comments/Comment.cshtml", comment);
        }

        [HttpPost]
        [Route("DeleteComment")]
        public async Task<IActionResult> Delete(string id)
        {
            var comment = await commentRepo.GetByIdAsync(id);
            if (comment == null) return View("/Views/Comments/CommentNotFound.cshtml");
            var post = await postRepo.GetAll().FirstOrDefaultAsync(p => p.Id == comment.PostId);

            await commentRepo.DeleteAsync(comment);
            return View("/Views/Posts/Post.cshtml", post);
        }
    }
}
