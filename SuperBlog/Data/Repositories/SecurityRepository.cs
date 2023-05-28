using Microsoft.AspNetCore.Identity;
using SuperBlog.Models.Entities;

namespace SuperBlog.Data.Repositories
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public SecurityRepository(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // Метод для создания ролей и админа с модератором
        // Этот метод должен вызываться один раз для подготовки базы данных (например при входе пользователя в систему, как сейчас и делается)
        // После этого вызов метода должен быть закомментирован, а проект пересобран, чтобы не замедлять работу приложения
        // Для последующих запусков предполагается, что юзер-админ и все роли уже есть в базе данных
        public async Task CreateRoles()
        {
            string[] roleNames = { "admin", "moderator", "user" };
            IdentityResult roleResult;
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .AddJsonFile("appsettings.json")
            .Build();

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new Role(roleName));
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

        public IQueryable<Role> GetRoles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Role> GetUserRole(string userId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<User> GetUsers(string role)
        {
            throw new NotImplementedException();
        }
    }

    public interface ISecurityRepository
    {
        public Task CreateRoles();
        public IQueryable<Role> GetRoles();
        public IEnumerable<Role> GetUserRole(string userId);
        public IQueryable<User> GetUsers(string role);
    }
}
