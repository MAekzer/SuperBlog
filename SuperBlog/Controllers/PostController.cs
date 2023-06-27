using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperBlogData.Exceptions;
using SuperBlogData.Models.ViewModels;
using SuperBlog.Services;

namespace SuperBlog.Controllers
{
    public class PostController : Controller
    {
        private readonly PostHandler handler;
        private readonly ErrorHandler errorHandler;

        public PostController(PostHandler handler, ErrorHandler errorHandler)
        {
            this.handler = handler;
            this.errorHandler = errorHandler;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await handler.SetupCreate(User);
            return View("/Views/Posts/NewPost.cshtml", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            var result = await handler.HandleCreate(model, User);
            if (result.Success) return RedirectToAction("MyFeed", "User");
            return View("/Views/Posts/NewPost.cshtml", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var model = await handler.SetupUpdate(id, User);
                return View("/Views/Posts/EditPost.cshtml", model);
            }
            catch (PostNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "post");
                return View("/Views/Error/PostNotFound.cshtml", errorModel);
            }
            catch (AccessDeniedException)
            {
                return RedirectToAction("AccessDenied", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(EditPostViewModel model)
        {
            try
            {
                var result = await handler.HandleUpdate(model, User);
                if (result.Success) return RedirectToAction("MyFeed", "User");
                return View("/Views/Posts/EditPost.cshtml", model);
            }
            catch (PostNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(model.Id, User, Response, "post");
                return View("/Views/Error/PostNotFound.cshtml", errorModel);
            }
            catch (AccessDeniedException)
            {
                return RedirectToAction("AccessDenied", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await handler.HandleDelete(id, User);
                return RedirectToAction("Posts", "Post");
            }
            catch (PostNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "post");
                return View("/Views/Error/PostNotFound.cshtml", errorModel);
            }
            catch (AccessDeniedException)
            {
                return RedirectToAction("AccessDenied", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Posts()
        {
            var model = await handler.SetupPosts(User);
            return View("/Views/Posts/AllPosts.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Posts(PostsViewModel model)
        {
            model = await handler.SetupPosts(model, User);
            return View("/Views/Posts/AllPosts.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Post(Guid id)
        {
            try
            {
                var model = await handler.SetupPost(id, User);
                return View("/Views/Posts/Post.cshtml", model);
            }
            catch (PostNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "post");
                return View("/Views/Error/PostNotFound.cshtml", errorModel);
            }
        }
    }
}
