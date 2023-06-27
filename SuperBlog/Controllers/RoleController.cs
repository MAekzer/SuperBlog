using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperBlogData.Exceptions;
using SuperBlogData.Models.ViewModels;
using SuperBlog.Services;

namespace SuperBlog.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleHandler handler;
        private readonly ErrorHandler errorHandler;

        public RoleController(RoleHandler roleHandler, ErrorHandler errorHandler)
        {
            handler = roleHandler;
            this.errorHandler = errorHandler;
        }

        [HttpGet]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Roles()
        {
            var model = await handler.SetupRoles();
            return View("/Views/Roles/Roles.cshtml", model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create()
        {
            return View("Views/Roles/CreateRole.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid) return View("/Views/Roles/CreateRole.cshtml", model);

            var result = await handler.HandleCreate(model);
            if (result.AlreadyExists)
            {
                ModelState.AddModelError("Name", "Роль с таким именем уже существует");
                return View("Views/Roles/CreateRole.cshtml", model);
            }
            return RedirectToAction("Roles", "Role");
        }

        [HttpGet]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Role(string id)
        {
            try
            {
                var model = await handler.SetupRole(id);
                return View("/Views/Roles/Role.cshtml", model);
            }
            catch (Exception ex) when (ex is RoleNotFoundException || ex is FormatException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "role");
                return View("/Views/Error/RoleNotFound.cshtml", errorModel);
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                var model = await handler.SetupUpdate(id);
                return View("/Views/Roles/EditRole.cshtml", model);
            }
            catch (Exception ex) when (ex is RoleNotFoundException || ex is FormatException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "role");
                return View("/Views/Error/RoleNotFound.cshtml", errorModel);
            }
            
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(EditRoleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View("/Views/Roles/EditRole.cshtml", model);
                var result = await handler.HandleUpdate(model);
                if (result.AlreadyExists)
                {
                    ModelState.AddModelError("Name", "Роль с таким названием уже существует");
                    return View("/Views/Roles/EditRole.cshtml", model);
                }
                return RedirectToAction("Roles", "Role");
            }
            catch (Exception ex) when (ex is RoleNotFoundException || ex is FormatException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(model.Id, User, Response, "role");
                return View("/Views/Error/RoleNotFound.cshtml", errorModel);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await handler.HandleDelete(id);
                if (result.Success) return RedirectToAction("Roles", "Role");
                throw new Exception();
            }
            catch (Exception ex) when (ex is RoleNotFoundException || ex is FormatException)
            {
                var errorModel = await errorHandler.HandleNotFoundError(id, User, Response, "role");
                return View("/Views/Error/RoleNotFound.cshtml", errorModel);
            }
        }
    }
}
