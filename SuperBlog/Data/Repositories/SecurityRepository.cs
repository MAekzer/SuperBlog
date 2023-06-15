using Microsoft.AspNetCore.Identity;
using SuperBlog.Models.Entities;
using System.Security.Claims;
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
            List<Role> roles = new();

            var admin = new Role("admin", "Администратор",
                "Высшая роль в приложении. Имеет право на все операции с любыми сущностями"
            );
            roles.Add( admin );

            var moderator = new Role("moderator", "Модератор",
                "Роль модератора приложения. Имеет право на удаление и редактирование любых статей и комментариев"
            );
            roles.Add( moderator );

            var user = new Role("user", "Пользователь",
                "Стандартная роль в приложении. Имеет право на операции со своими статьями, комментариями и профилем"
            );
            roles.Add( user );

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

        public async Task<bool> IsSameUserOrModerator(ClaimsPrincipal User, Guid CompareId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return false;
            if (user.Id == CompareId) return true;
            if (User.IsInRole("moderator")) return true;
            return false;
        }

        public async Task<bool> IsSameUserOrAdmin(ClaimsPrincipal User, Guid CompareId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return false;
            if (user.Id == CompareId) return true;
            if (User.IsInRole("admin")) return true;
            return false;
        }
    }

    public interface ISecurityRepository
    {
        public Task CreateRoles();
        public Task<bool> IsSameUserOrModerator(ClaimsPrincipal User, Guid CompareId);
        public Task<bool> IsSameUserOrAdmin(ClaimsPrincipal User, Guid CompareId);
    }
}
