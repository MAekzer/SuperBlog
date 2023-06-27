using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperBlogData.Exceptions;
using SuperBlogData.Models.ViewModels;
using SuperBlog.Services;

namespace SuperBlog.Controllers
{
    public class CommentController : Controller
    {
        private readonly CommentHandler handler;
        private readonly ErrorHandler errorHandler;

        public CommentController(
            CommentHandler handler,
            ErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
            this.handler = handler;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var model = await handler.SetupEdit(id, User);
                return View("/Views/Comments/EditComment.cshtml", model);
            }
            catch (CommentNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "comment");
                return View("/Views/Error/CommentNotFound.cshtml", errorModel);
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

        [HttpPost]
        public async Task<IActionResult> Edit(EditCommentViewModel model)
        {
            try
            {
                var result = await handler.HandleEdit(model, User);
                return RedirectToAction("Post", "Post", new { id = result.PostId });
            }
            catch (CommentNotFoundException)
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
        [HttpGet]
        public async Task<IActionResult> Write(Guid postId)
        {
            try
            {
                var model = await handler.SetupWrite(postId);
                return View("/Views/Comments/WriteComment.cshtml", model);

            }
            catch (PostNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(postId, User, Response, "post");
                return View("/Views/Error/PostNotFound.cshtml", errorModel);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Write(WriteCommentViewModel model)
        {
            try
            {
                var result = await handler.HandleWrite(model, User);
                return RedirectToAction("Post", "Post", new { id = result.PostId });
            }
            catch (PostNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(model.PostId, User, Response, "post");
                return View("/Views/Error/PostNotFound.cshtml", errorModel);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await handler.HandleDelete(id, User);
                return RedirectToAction("Post", "Post", new { id = result.PostId });
            }
            catch (CommentNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "post");
                return View("/Views/Error/PostNotFound.cshtml", errorModel);
            }
            catch (AccessDeniedException)
            {
                return RedirectToAction("AccessDenied", "Home");
            }
        }
    }
}
