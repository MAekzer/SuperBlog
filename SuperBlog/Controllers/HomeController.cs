using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperBlog.Models;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;
using System.Diagnostics;

namespace SuperBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> userManager;

        public HomeController(UserManager<User> _userManager)
        {
            userManager = _userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null) return RedirectToAction("MyFeed", "User");

            return View();
        }

        [HttpGet]
        [Route("AccessDenied")]
        public async Task<IActionResult> AccessDenied()
        {
            return View("/Views/Admin/AccessDenied.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}