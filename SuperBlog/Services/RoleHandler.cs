using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Data.Repositories;
using SuperBlog.Exceptions;
using SuperBlog.Extentions;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;
using SuperBlog.Services.Results;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace SuperBlog.Services
{
    public class RoleHandler
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IRepository<Post> postRepo;
        private readonly ILogger<RoleHandler> logger;

        public RoleHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IRepository<Post> postRepo, ILogger<RoleHandler> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.postRepo = postRepo;
            this.logger = logger;
        }

        public async Task<RolesViewModel> SetupRoles()
        {
            var roles = await roleManager.Roles.ToListAsync();
            var model = new RolesViewModel { Roles = roles };
            return model;
        }

        public async Task<RoleHandlingResult> HandleCreate(CreateRoleViewModel model)
        {
            var result = new RoleHandlingResult();
            var alreadyExists = await roleManager.RoleExistsAsync(model.Name);
            if (alreadyExists)
            {
                result.AlreadyExists = true;
                return result;
            }
            var role = new Role(model.Name, model.DisplayName, model.Description);
            await roleManager.CreateAsync(role);
            result.Success = true;
            return result;
        }

        public async Task<RoleViewModel> SetupRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id) ?? throw new RoleNotFoundException();
            var users = await userManager.GetUsersInRoleAsync(role.Name);
            var model = new RoleViewModel { Role = role, Users = new List<UserViewModel>() };

            foreach (var user in users)
            {
                var roles = await roleManager.GetRoles(user, userManager);
                int count = await postRepo.GetAll().Where(p => p.UserId == user.Id).CountAsync();
                var userModel = new UserViewModel { User = user, Roles = roles, PostCount = count };
                model.Users.Add(userModel);
            }
            return model;
        }

        public async Task<EditRoleViewModel> SetupUpdate(string id)
        {
            var role = await roleManager.FindByIdAsync(id) ?? throw new RoleNotFoundException();
            var model = new EditRoleViewModel { Id = new Guid(id), Name = role.Name, DisplayName = role.DisplayName, Description = role.Description };
            return model;
        }

        public async Task<RoleHandlingResult> HandleUpdate(EditRoleViewModel model)
        {
            var result = new RoleHandlingResult();
            var role = await roleManager.FindByIdAsync(model.Id.ToString()) ?? throw new RoleNotFoundException();
            var existingRole = await roleManager.FindByNameAsync(model.Name);
            if (existingRole != null && existingRole.Id != model.Id)
            {
                result.AlreadyExists = true;
                return result;
            }
            role.Update(model);
            await roleManager.UpdateAsync(role);
            result.Success = true;
            return result;
        }

        public async Task<RoleHandlingResult> HandleDelete(string id)
        {
            var result = new RoleHandlingResult();
            var role = await roleManager.FindByIdAsync(id) ?? throw new RoleNotFoundException();
            await roleManager.DeleteAsync(role);
            result.Success = true;
            return result;
        }
    }
}
