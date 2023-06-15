using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;

namespace SuperBlog.Controllers
{
    public class RoleController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public RoleController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Roles()
        {
            var roles = await roleManager.Roles.ToListAsync();
            var model = new RolesViewModel { Roles = roles };
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

            var newRole = new Role(model.Name, model.DisplayName, model.Description);
            await roleManager.CreateAsync(newRole);
            return RedirectToAction("Roles", "Role");
        }

        [HttpGet]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Role(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null) return View("Error");
            var users = await userManager.GetUsersInRoleAsync(role.Name);
            var model = new RoleViewModel { Role = role, Users = users.ToList() };
            return View("/Views/Roles/Role.cshtml", model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(Guid id)
        {
            var role = await roleManager.FindByIdAsync(id.ToString());
            if (role == null) return View("Error");
            var model = new EditRoleViewModel { Id = id, Name = role.Name, DisplayName = role.DisplayName, Description = role.Description };
            return View("/Views/Roles/EditRole.cshtml", model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(EditRoleViewModel model)
        {
            if (!ModelState.IsValid) return View("/Views/Roles/EditRole.cshtml", model);

            var role = await roleManager.FindByIdAsync(model.Id.ToString());
            if (role == null) return View("Error");
            role.Update(model);
            await roleManager.UpdateAsync(role);
            return RedirectToAction("Roles", "Role");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role != null) await roleManager.DeleteAsync(role);
            return RedirectToAction("Roles", "Role");
        }
    }
}
