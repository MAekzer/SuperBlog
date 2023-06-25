using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Data.Repositories;
using SuperBlog.Exceptions;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;
using SuperBlog.Services;

namespace SuperBlog.Controllers
{
    public class TagController : Controller
    {
        private readonly ErrorHandler errorHandler;
        private readonly TagHandler handler;

        public TagController(ErrorHandler errorHandler, TagHandler handler)
        {
            this.errorHandler = errorHandler;
            this.handler = handler;
        }

        [HttpGet]
        public async Task<IActionResult> AllTags()
        {
            var model = await handler.SetupTags();
            return View("/Views/Tags/AllTags.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Tag(Guid id)
        {
            try
            {
                var model = await handler.SetupTag(id, User);
                return View("/Views/Tags/TagPosts.cshtml", model);
            }
            catch (TagNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "tag");
                return View("/Views/Error/TagNotFound.cshtml", errorModel);
            }
        }

        [HttpPost]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Create(TagViewModel model)
        {
            var result = await handler.HandleCreate(model);
            if (result.AlreadyExists)
            {
                ModelState.AddModelError("Name", "Тег с таким именем уже существует");
                return View("/Views/Tags/AllTags.cshtml", model);
            }
            return RedirectToAction("AllTags", "Tag");
        }

        [HttpGet]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var model = await handler.SetupUpdate(id);
                return View("/Views/Tags/EditTag.cshtml", model);
            }
            catch (TagNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "tag");
                return View("/Views/Error/TagNotFound.cshtml", errorModel);
            }
        }

        [HttpPost]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Update(EditTagViewModel model)
        {
            try
            {
                var result = await handler.HandleUpdate(model);
                return RedirectToAction("AllTags", "Tag");
            }
            catch (TagNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(model.Id, User, Response, "tag");
                return View("/Views/Error/TagNotFound.cshtml", errorModel);
            }
        }

        [HttpPost]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await handler.HandleDelete(id);
                return RedirectToAction("AllTags", "Tag");
            }
            catch (TagNotFoundException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "tag");
                return View("/Views/Error/TagNotFound.cshtml", errorModel);
            }
            
        }
    }
}
