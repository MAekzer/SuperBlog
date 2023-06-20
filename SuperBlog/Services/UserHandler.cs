using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
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

        {
            var result = new UserHandlingResult();
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null) 
            {
                result.IncorrectLoginOrPassword = true;
                return result;
            };
            var signInResult = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (signInResult.Succeeded)
            {
                result.Success = true;
                return result;
            }
            result.IncorrectLoginOrPassword = true;
            return result;
        }

        public async Task<UserHandlingResult> HandleRegister(RegisterViewModel model)
        {
            var result = new UserHandlingResult();

            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                result.EmailAlreadyExists = true;
                return result;
            }

            existingUser = await userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
            {
                result.LoginAlreadyExists = true;
                return result;
            }

            await signInManager.SignOutAsync();
            var user = mapper.Map<User>(model);
            var createUserResult = await userManager.CreateAsync(user, model.Password);

            if (createUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "user");
                await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                result.Success = true;
            }
            return result;
        }

        public async Task<EditUserViewModel> SetupUpdate(string id)
        {
            var user = await userManager.FindByIdAsync(id) ?? throw new UserNotFoundException();
            var model = mapper.Map<EditUserViewModel>(user);

            var roles = roleManager.Roles;
            var userRoles = await roleManager.GetRoles(user, userManager);

            foreach (var role in roles)
            {
                var roleModel = new RoleCheckboxViewModel { DispayName = role.DisplayName, Id = role.Id, IsChecked = false };
                if (userRoles.Any(r => r.Id == role.Id))
                    roleModel.IsChecked = true;
                model.Roles.Add(roleModel);
            }

            return model;
        }

        public async Task<UserHandlingResult> HandleUpdate(EditUserViewModel model)
        {
            var result = new UserHandlingResult();
            var user = await userManager.FindByIdAsync(model.Id.ToString()) ?? throw new UserNotFoundException();
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                result.EmailAlreadyExists = true;
                return result;
            }

            existingUser = await userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
            {
                result.LoginAlreadyExists = true;
                return result;
            }

            user.Update(model);

            foreach (var roleModel in model.Roles)
            {
                var role = await roleManager.Roles.FirstOrDefaultAsync(r => r.Id == roleModel.Id);
                if (roleModel.IsChecked || roleModel.DispayName == "Пользователь")
                    await userManager.AddToRoleAsync(user, role.Name);
                else
                    await userManager.RemoveFromRoleAsync(user, role.Name);
            }

            await userManager.UpdateAsync(user);
            result.Success = true;
            return result;
        }

        public async Task<UserPostsViewModel> SetupMyFeed(ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal) ?? throw new UserNotFoundException();
            var posts = (await postRepo.GetAll().Include(p => p.Tags).Include(p => p.User)
                .Where(p => p.UserId == user.Id).ToListAsync()).OrderBy(p => p.GetTime()).ToList();
            var model = new UserPostsViewModel { User = user, Posts = posts };
            return model;
        }

        public async Task<UserViewModel> SetupProfile(ClaimsPrincipal principal, string id = "")
        {
            User user;
            var currentUser = await userManager.GetUserAsync(principal) ?? throw new UserNotFoundException();

            if (string.IsNullOrEmpty(id))
                user = currentUser;
            else 
            {
                user = await userManager.FindByIdAsync(id) ?? throw new UserNotFoundException();
                if (user.Id == currentUser.Id)
                {
                    return new UserViewModel();
                }
            }

            var roles = await roleManager.GetRoles(user, userManager);
            var posts = await postRepo.GetAll().Include(p => p.Tags).Where(p => p.UserId == user.Id).ToListAsync();
            return new UserViewModel { User = user, Posts = posts, Roles = roles, PostCount = posts.Count };
        }

        public async Task<UserHandlingResult> HandleDelete(string id, ClaimsPrincipal principal)
        {
            var result = new UserHandlingResult();
            var user = await userManager.FindByIdAsync(id);
            var currentUser = await userManager.GetUserAsync(principal);
            if (user == null) throw new UserNotFoundException();
            if (currentUser == null || !principal.IsInRole("admin"))
            {
                result.AccessDenied = true;
                return result;
            }
            if (user.Id == currentUser.Id) await signInManager.SignOutAsync();
            await userManager.DeleteAsync(user);
            result.Success = true;
            return result;
        }

        public async Task<UsersViewModel> SetupUsers()
        {
            var model = new UsersViewModel();
            var users = userManager.Users;
            foreach (var user in users)
            {
                int postCount = await postRepo.GetAll().Where(p => p.User.Id == user.Id).CountAsync();
                var roles = await roleManager.GetRoles(user, userManager);
                var userModel = new UserViewModel { User = user, PostCount = postCount, Roles = roles };
                model.Users.Add(userModel);
            }
            return model;
        }

        public async Task<UsersViewModel> SetupSearch(UsersViewModel model)
        {
            var users = userManager.Users;
            var param = model.SearchParam.ToString();
            switch (model.SearchCriterion)
            {
                case SearchCriteria.Имя:
                    users = users.Where(u => u.FirstName.Contains(param) || u.LastName.Contains(param) || u.MiddleName.Contains(param));
                    break;
                case SearchCriteria.Email:
                    users = users.Where(u => u.Email.Contains(param));
                    break;
                case SearchCriteria.Логин:
                    users = users.Where(u => u.UserName.Contains(param));
                    break;
            }
            model.Users.Clear();
            foreach (var user in users)
            {
                int postCount = await postRepo.GetAll().Where(p => p.User.Id == user.Id).CountAsync();
                var roles = await roleManager.GetRoles(user, userManager);
                var userModel = new UserViewModel { User = user, PostCount = postCount, Roles = roles };
                model.Users.Add(userModel);
            }
            return model;
        }

        public async Task<UserHandlingResult> HandleLogout()
        {
            var result = new UserHandlingResult();
            await signInManager.SignOutAsync();
            result.Success = true;
            return result;
        }
    }
}
