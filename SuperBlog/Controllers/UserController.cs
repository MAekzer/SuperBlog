using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Data.Repositories;
using SuperBlog.Exceptions;
using SuperBlog.Extentions;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;
using SuperBlog.Services;

namespace SuperBlog.Controllers
{
    public class UserController : Controller
    {
        private readonly UserHandler handler;

        public UserController(UserHandler _handler)
        {
            handler = _handler;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Неверный фомат ввода");
                return View("/Views/Home/Index.cshtml", model);
            }
            var result = await handler.HandleLogin(model);
            if (result.Success) return RedirectToAction("MyFeed", "User");
            ModelState.AddModelError("", "Неверный логин или пароль");
            return View("/Views/Home/Index.cshtml", model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("/Views/Home/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await handler.HandleRegister(model);
            if (result.EmailAlreadyExists)
            {
                ModelState.AddModelError("Email", "Пользователь с таким адресом электронной почты уже существует");
                return View("/Views/Home/Register.cshtml", model);
            }

            if (result.LoginAlreadyExists)
            {
                ModelState.AddModelError("UserName", "Пользователь с таким именем уже существует");
                return View("/Views/Home/Register.cshtml", model);
            }

            if (result.Success) return RedirectToAction("MyFeed", "User");

            return View("/Views/Home/Register.cshtml", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                var model = await handler.SetupUpdate(id);
                return View("/Views/Users/EditProfile.cshtml", model);
            }
            catch (UserNotFoundException)
            {
                return View("/Views/Error/UserNotFound.cshtml");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(EditUserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View("/Views/Users/EditProfile.cshtml", model);

                var result = await handler.HandleUpdate(model);
                if (result.LoginAlreadyExists)
                {
                    ModelState.AddModelError("Email", "Пользователь с таким именем уже существует");
                    return View("/Views/Users/EditProfile.cshtml", model);
                }
                if (result.EmailAlreadyExists)
                {
                    ModelState.AddModelError("Email", "Пользователь с таким адресом электронной почты уже существует");
                    return View("/Views/Users/EditProfile.cshtml", model);
                }
                if (result.Success) return RedirectToAction("UserProfile", "User", new { id = model.Id });
                return View("/Views/Users/EditProfile.cshtml");
            }
            catch (UserNotFoundException)
            {
                return View("/Views/Error/UserNotFound.cshtml");
            }
            
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyFeed()
        {
            try
            {
                var model = await handler.SetupMyFeed(User);
                return View("/Views/User/MyFeed.cshtml", model);
            }
            catch (UserNotFoundException)
            {
                return View("/Views/Error/UserNotFound.cshtml");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            try
            {
                var model = await handler.SetupProfile(User);
                return View("/Views/Users/MyProfile.cshtml", model);
            }
            catch (UserNotFoundException)
            {
                return View("/Views/Error/UserNotFound.cshtml");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await handler.HandleDelete(id, User);
                if (result.AccessDenied) return View("/Views/Error/AccessDenied.cshtml");
                return RedirectToAction("Users", "User");
            }
            catch(UserNotFoundException)
            {
                return View("/Views/Error/UserNotFoundException.cshtml");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var model = await handler.SetupUsers();
            return View("/Views/Users/Users.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Users(UsersViewModel model)
        {
            if (model == null || model.SearchCriterion == null || string.IsNullOrEmpty(model.SearchParam))
                return RedirectToAction("Users", "User");
            model = await handler.SetupSearch(model);
            return View("/Views/Users/Users.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(string id)
        {
            try
            {
                var model = await handler.SetupProfile(User);
                if (model is null)
                    return RedirectToAction("MyProfile", "User");
                return View("/Views/Users/UserProfile.cshtml", model);
            }
            catch (UserNotFoundException)
            {
                return View("/Views/Error/UserNotFound.cshtml");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await handler.HandleLogout();
            return RedirectToAction("Index", "Home");
        }
    }
}
