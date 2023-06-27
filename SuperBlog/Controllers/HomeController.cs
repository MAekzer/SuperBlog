using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.ViewModels;
using System.Diagnostics;

namespace SuperBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<HomeController> logger;

        public HomeController(UserManager<User> _userManager, ILogger<HomeController> logger)
        {
            userManager = _userManager;
            this.logger = logger;
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
            var user = await userManager.GetUserAsync(User);
            if (user != null)
                logger.LogError($"Access denied for request {HttpContext.TraceIdentifier} from user {user.UserName} with id {user.Id}");
            else
                logger.LogError($"Access denied for request {HttpContext.TraceIdentifier} from anonimous user");

            return View("/Views/Error/AccessDenied.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = false)]
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var model = new ErrorViewModel 
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier ,
                Path = exceptionDetails.Path,
                Time = DateTime.UtcNow
            };
            logger.LogError($"Unhandled error. Request ID: {model.RequestId}. Original path: {model.Path}");

            return View("/Views/Error/Error.cshtml", model);
        }

        [Route("Error/{statusCode}")]
        public IActionResult StatusCodeError(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    var exceptionDetails = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
                    var model = new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                        Path = exceptionDetails?.OriginalPath,
                        Time = DateTime.UtcNow
                    };
                    logger.LogError($"Error {statusCode}. Request ID: {model.RequestId}. Original path: {model.Path}");
                    return View("/Views/Error/ResourceNotFound.cshtml", model);
                default:
                    return RedirectToAction("StatusCodeUnhandledError", "Home", new { statusCode });
            }
        }

        public IActionResult StatusCodeUnhandledError(int statusCode)
        {
            var details = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Path = details.OriginalPath,
                Time = DateTime.UtcNow
            };
            logger.LogError($"Unhandled error with status code {statusCode}. Request ID: {model.RequestId}. Original path: {model.Path}");
            return View("/Views/Error/Error.cshtml", model);
        }
    }
}