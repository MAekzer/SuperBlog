using Microsoft.AspNetCore.Identity;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.ViewModels;
using System.Security.Claims;

namespace SuperBlog.Services
{
    public class ErrorHandler
    {
        private readonly ILogger<ErrorHandler> logger;
        private readonly UserManager<User> userManager;

        public ErrorHandler(ILogger<ErrorHandler> logger, UserManager<User> userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<EntityNotFoundViewModel> HandleNotFoundError(Guid id, ClaimsPrincipal principal, HttpResponse response, string entityName)
        {
            response.StatusCode = 404;
            var currentUser = await userManager.GetUserAsync(principal);
            string message = $"Attempt to access non-existent {entityName} with id {id}";
            if (currentUser != null)
                message += $" by user with login {currentUser.UserName} with id {currentUser.Id}.";
            else
                message += " by anonimous user.";
            logger.LogError(message);
            return new EntityNotFoundViewModel { Id = id.ToString() };
        }

        public async Task<EntityNotFoundViewModel> HandleNotFoundError(string id, ClaimsPrincipal principal, HttpResponse response, string entityName)
        {
            response.StatusCode = 404;
            var currentUser = await userManager.GetUserAsync(principal);
            string message = $"Attempt to access non-existent {entityName} with id {id}";
            if (currentUser != null)
                message += $" by user with login {currentUser.UserName} with id {currentUser.Id}.";
            else
                message += " by anonimous user.";
            logger.LogError(message);
            return new EntityNotFoundViewModel { Id = id };
        }
    }
}
