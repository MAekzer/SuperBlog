using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SuperBlog.Data.Repositories;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;

namespace SuperBlog.Services
{
    public class UserHandler
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IMapper mapper;
        private readonly ISecurityRepository security;
        private readonly IRepository<Post> postRepo;

        public UserHandler(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IMapper mapper,
            ISecurityRepository security,
            IRepository<Post> postRepo)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.security = security;
            this.postRepo = postRepo;
        }

        public async Task<bool> HandleLogin(LoginViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null) return false;
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (result.Succeeded) return true;
            return false;
        }

        public async Task<bool> HandleRegister(RegisterViewModel model)
        {
            await signInManager.SignOutAsync();
            var user = mapper.Map<User>(model);
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "user");
                await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                return true;
            }
            return false;
        }
    }
}
