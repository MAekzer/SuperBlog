using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Data.Repositories;
using SuperBlog.Extentions;
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
        public async Task<IActionResult> Edit(Guid id)
        {
            var comment = await commentRepo.GetByIdAsync(id);
            if (comment == null) return View("/Views/Comments/CommentNotFound.cshtml");

            var post = await postRepo.GetAll().Include(p => p.User).Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == comment.PostId);
            var postmodel = mapper.Map<PostViewModel>(post);

            var model = new EditCommentViewModel { Content = comment.Content, Id =  comment.Id, Post = postmodel};

            return View("/Views/Comments/EditComment.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCommentViewModel model)
        {
            var comment = await commentRepo.GetByIdAsync(model.Id);
            if (comment == null) return View("/Views/Error/CommentNotFound.cshtml");

            comment.Update(model);
            await commentRepo.UpdateAsync(comment);

            return RedirectToAction("Post", "Post", new { id = comment.PostId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Write(Guid postId)
        {
            var post = await postRepo.GetAll().Include(p => p.User).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null) 
                return RedirectToAction("Index", "Home");

            var postModel = mapper.Map<PostViewModel>(post);
            var model = new WriteCommentViewModel { Post = postModel };

            return View("/Views/Comments/WriteComment.cshtml", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Write(WriteCommentViewModel model)
        {
            var post = await postRepo.GetByIdAsync(model.PostId);
            var user = await userManager.GetUserAsync(User);
            if (post == null || user == null) return RedirectToAction("Index", "Home");

            var comment = new Comment 
            {
                Content = model.Content,
                Post = post,
                User = user
            };

            await commentRepo.AddAsync(comment);
            return RedirectToAction("Post", "Post", new { id = model.PostId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var comment = await commentRepo.GetByIdAsync(id);
            if (comment == null) return View("/Views/Error/CommentNotFound.cshtml");

            var postId = comment.PostId;

            await commentRepo.DeleteAsync(comment);
            return RedirectToAction("Post", "Post", new { id = postId });
        }
    }
}
