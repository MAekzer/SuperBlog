using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;
using SuperBlog.Extentions;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Data.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace SuperBlog.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IMapper mapper;
        private readonly ISecurityRepository security;
        private readonly IRepository<Post> postRepo;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper,
            ISecurityRepository security,
            IRepository<Post> postRepo,
            RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.security = security;
            this.postRepo = postRepo;
            this.roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //await security.CreateRoles();
                var user = await userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                    return View(@"\Views\Home\Index.cshtml", model);
                }

                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("MyFeed", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                    return View(@"\Views\Home\Index.cshtml", model);
                }
            }
            ModelState.AddModelError("", "Неверный формат ввода");
            return View(@"\Views\Home\Index.cshtml", model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("/Views/Home/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                await signInManager.SignOutAsync();
                var user = mapper.Map<User>(model);
                var existingUser = await userManager.FindByEmailAsync(user.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Пользователь с таким адресом электронной почты уже существует");
                    return View(@"\Views\Home\Register.cshtml", model);
                }

                existingUser = await userManager.FindByNameAsync(user.UserName);

                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "Пользователь с таким логином уже существует");
                    return View(@"\Views\Home\Register.cshtml", model);
                }

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "user");
                    await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    return RedirectToAction("MyFeed", "User");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(@"\Views\Home\Register.cshtml", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return RedirectToAction("Index", "Home");

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

            return View("/Views/Users/EditProfile.cshtml", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return View("/Views/Users/EditProfile", model);

            var user = await userManager.FindByIdAsync(model.Id.ToString());
            if (user == null) return RedirectToAction("Index", "Home");

            var existingUser = await userManager.FindByEmailAsync(model.Email);

            if (existingUser != null && existingUser.Id != model.Id)
            {
                ModelState.AddModelError("Email", "Пользователь с таким адресом электронной почты уже существует");
                return View(@"\Views\Users\EditProfile.cshtml", model);
            }

            existingUser = await userManager.FindByNameAsync(model.UserName);

            if (existingUser != null && existingUser.Id != model.Id)
            {
                ModelState.AddModelError("UserName", "Пользователь с таким логином уже существует");
                return View(@"\Views\Users\EditProfile.cshtml", model);
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
            return RedirectToAction("UserProfile", "User", new {id = model.Id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyFeed()
        {
            var _user = User;
            if (_user != null)
            {
                var user = await userManager.GetUserAsync(_user);
                var posts = (await postRepo.GetAll().Include(p => p.Tags).Include(p => p.User)
                    .Where(p => p.UserId == user.Id).ToListAsync()).OrderBy(p => p.GetTime()).ToList();
                if (user != null)
                {
                    var model = new UserPostsViewModel { User = user, Posts = posts };
                    return View("/Views/Users/MyFeed.cshtml", model);
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return View("/View/Error/UserNotFound");

            var roles = await roleManager.GetRoles(user, userManager);

            var posts = await postRepo.GetAll().Include(p => p.Tags).Where(p => p.UserId == user.Id).ToListAsync();
            var model = new UserViewModel { User = user, Posts = posts, Roles = roles, PostCount = posts.Count };

            return View("/Views/Users/MyProfile.cshtml", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var currentUser = await userManager.GetUserAsync(User);

            if (currentUser == null || user == null) return View("/Views/Error/UserNotFound.cshtml");

            if (currentUser.Id == user.Id)
            {
                await signInManager.SignOutAsync();
                await userManager.DeleteAsync(user);
                return RedirectToAction("Index", "Home");
            }

            await userManager.DeleteAsync(user);
            return View("/Views/Users/Users.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = userManager.Users;
            var model = new UsersViewModel();

            foreach (var user in users)
            {
                int postCount = await postRepo.GetAll().Where(p => p.User.Id == user.Id).CountAsync();
                var roles = await roleManager.GetRoles(user, userManager);
                var userModel = new UserViewModel { User = user, PostCount = postCount, Roles = roles};
                model.Users.Add(userModel);
            }

            return View("/Views/Users/Users.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Users(UsersViewModel model)
        {
            if (model == null || model.SearchCriterion == null || string.IsNullOrEmpty(model.SearchParam))
                return RedirectToAction("Users", "User");

            var users = userManager.Users;
            var param = model.SearchParam.ToString();
            switch(model.SearchCriterion)
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

            foreach (var user in users)
            {
                int postCount = await postRepo.GetAll().Where(p => p.User.Id == user.Id).CountAsync();
                var roles = await roleManager.GetRoles(user, userManager);
                var userModel = new UserViewModel { User = user, PostCount = postCount, Roles = roles };
                model.Users.Add(userModel);
            }

            return View("/Views/Users/Users.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(string id) 
        {
            var currentUser = await userManager.GetUserAsync(User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null) return View("/View/Error/UserNotFound");
            if (currentUser != null && (currentUser.Id == user.Id))
                return RedirectToAction("MyProfile", "User");

            var roles = await roleManager.GetRoles(user, userManager);

            var posts = await postRepo.GetAll().Include(p => p.Tags).Where(p => p.UserId == user.Id).ToListAsync();
            var model = new UserViewModel { User = user, Posts = posts, Roles = roles, PostCount = posts.Count };

            return View("/Views/Users/UserProfile.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
