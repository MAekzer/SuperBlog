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
        private readonly IMapper mapper;
        private readonly ISecurityRepository security;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, ISecurityRepository security)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.security = security;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                await security.CreateRoles();
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
        [Route("Register")]
        public IActionResult Register()
        {
            return View("/Views/Home/Register.cshtml");
        }

        [HttpPost]
        [Route("Register")]
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

        [HttpGet]
        [Route("UpdateUser")]
        public async Task<IActionResult> Update()
        {
            var _user = User;
            if (_user == null) return RedirectToAction("Index", "Home");

            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            var model = mapper.Map<UpdateUserViewModel>(user);
            return View("/Views/Users/EditProfile.cshtml", model);
        }

        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IActionResult> Update(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid) return View("/Views/Users/EditProfile", model);

            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            user.Update(model);
            var newModel = mapper.Map<UpdateUserViewModel>(user);
            return View("/Views/Users/EditProfile/cshtml", newModel);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("MyFeed")]
        public async Task<IActionResult> MyFeed()
        {
            var _user = User;
            if (_user != null)
            {
                var user = await userManager.GetUserAsync(_user);
                if (user != null)
                {
                    var model = new UserViewModel { User = user };
                    return View("/Views/Users/MyFeed.cshtml", model);
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null) await userManager.DeleteAsync(user);

            return View("/Views/Admin/UserDeleted.cshtml", user);
        }

        [HttpGet]
        [Route("AllUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await userManager.Users.ToListAsync();

            return View("/Views/Users/Users.cshtml", users);
        }

        public async Task<IActionResult> GetUserById(string id) 
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return View("Views/Users/UserNotFound");

            return View("/Views/Users/UserProfile.cshtml", user);
        }

    }
}
