using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperBlogData.Repositories;
using SuperBlogData.Exceptions;
using SuperBlogData.Extentions;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.ViewModels;
using SuperBlog.Services.Results;

namespace SuperBlog.Services
{
    public class RoleHandler
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IRepository<Post> postRepo;

        public RoleHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IRepository<Post> postRepo)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.postRepo = postRepo;
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

        public async Task CreateRoles()
        {
            List<Role> roles = new();

            var admin = new Role("admin", "Администратор",
                "Высшая роль в приложении. Имеет право на все операции с любыми сущностями"
            );
            roles.Add(admin);

            var moderator = new Role("moderator", "Модератор",
                "Роль модератора приложения. Имеет право на удаление и редактирование любых статей и комментариев"
            );
            roles.Add(moderator);

            var user = new Role("user", "Пользователь",
                "Стандартная роль в приложении. Имеет право на операции со своими статьями, комментариями и профилем"
            );
            roles.Add(user);

            IdentityResult roleResult;
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .AddJsonFile("appsettings.json")
            .Build();

            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role.Name);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(role);
                }
            }

            var poweruser = new User
            {
                FirstName = configuration["AdminCredentials:FirstName"],
                LastName = configuration["AdminCredentials:LastName"],
                UserName = configuration["AdminCredentials:UserName"],
                Email = configuration["AdminCredentials:Email"],
            };

            string userPWD = configuration["AdminCredentials:Password"];
            var _user = await userManager.FindByNameAsync(configuration["AdminCredentials:UserName"]);

            if (_user == null)
            {
                var createPowerUser = await userManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRolesAsync(poweruser, new string[] { "admin", "moderator", "user" });
                }
            }

            poweruser = new User
            {
                FirstName = configuration["ModeratorCredentials:FirstName"],
                LastName = configuration["ModeratorCredentials:LastName"],
                UserName = configuration["ModeratorCredentials:UserName"],
                Email = configuration["ModeratorCredentials:Email"],
            };

            userPWD = configuration["ModeratorCredentials:Password"];
            _user = await userManager.FindByNameAsync(configuration["ModeratorCredentials:UserName"]);

            if (_user == null)
            {
                var createPowerUser = await userManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRolesAsync(poweruser, new string[] { "moderator", "user" });
                }
            }
        }
    }
}
