using Microsoft.AspNetCore.Identity;
using SuperBlogData.Models.Entities;

namespace SuperBlogData.Extentions
{
    public static class RoleManagerExtentions
    {
        public static async Task<List<Role>> GetRoles(this RoleManager<Role> roleManager, User user, UserManager<User> userManager)
        {
            var userRoles = new List<Role>();
            var roles = roleManager.Roles;
            var roleNames = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                if (roleNames.Contains(role.Name))
                    userRoles.Add(role);
            }

            return userRoles;
        }

        public static async Task<List<Guid>> GetRoleIds(this RoleManager<Role> roleManager, User user, UserManager<User> userManager)
        {
            var userRoles = new List<Guid>();
            var roles = roleManager.Roles;
            var roleNames = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                if (roleNames.Contains(role.Name))
                    userRoles.Add(role.Id);
            }

            return userRoles;
        }
    }
}
