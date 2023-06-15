using Microsoft.AspNetCore.Identity;
using SuperBlog.Models.Entities;

namespace SuperBlog.Extentions
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
    }
}
