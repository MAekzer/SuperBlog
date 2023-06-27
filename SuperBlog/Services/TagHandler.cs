using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperBlogData.Repositories;
using SuperBlogData.Exceptions;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.ViewModels;
using SuperBlog.Services.Results;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace SuperBlog.Services
{
    public class TagHandler
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<TagHandler> logger;
        private readonly IRepository<Tag> tagRepo;

        public TagHandler(UserManager<User> userManager, ILogger<TagHandler> logger, IRepository<Tag> tagRepo)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.tagRepo = tagRepo;
        }

        public async Task<TagViewModel> SetupTags()
        {
            var tags = await tagRepo.GetAll().Include(t => t.Posts).ToListAsync();
            var dict = new Dictionary<Tag, int>();
            foreach (var tag in tags)
            {
                dict.Add(tag, tag.Posts.Count);
            }
            var model = new TagViewModel { Tags = dict };
            return model;
        }

        public async Task<TagPostsViewModel> SetupTag(Guid id, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            var tag = await tagRepo.GetAll().Include("Posts.User").Include("Posts.Tags").FirstOrDefaultAsync(t => t.Id == id)
                            ?? throw new TagNotFoundException();
            var model = new TagPostsViewModel { Name = tag.Name, User = user, Posts = tag.Posts };
            return model;
        }

        public async Task<TagHandlingResult> HandleCreate(TagViewModel model)
        {
            var result = new TagHandlingResult();
            var existingTag = await tagRepo.GetByNameAsync(model.Name);
            if (existingTag != null)
            {
                result.AlreadyExists = true;
                return result;
            }
            var tag = new Tag { Name = model.Name };
            await tagRepo.AddAsync(tag);
            result.Success = true;
            return result;
        }

        public async Task<EditTagViewModel> SetupUpdate(Guid id)
        {
            var tag = await tagRepo.GetByIdAsync(id) ?? throw new TagNotFoundException();
            var model = new EditTagViewModel { Id = id, Name = tag.Name };
            return model;
        }

        public async Task<TagHandlingResult> HandleUpdate(EditTagViewModel model)
        {
            var result = new TagHandlingResult();
            var tag = await tagRepo.GetByIdAsync(model.Id) ?? throw new TagNotFoundException();
            var existingTag = await tagRepo.GetByNameAsync(tag.Name);
            if (existingTag != null && existingTag.Id != tag.Id)
            {
                result.AlreadyExists = true;
                return result;
            }
            tag.Name = model.Name;
            await tagRepo.UpdateAsync(tag);
            result.Success = true;
            return result;
        }

        public async Task<TagHandlingResult> HandleDelete(Guid id)
        {
            var result = new TagHandlingResult();
            var tag = await tagRepo.GetByIdAsync(id) ?? throw new TagNotFoundException();
            await tagRepo.DeleteAsync(tag);
            result.Success = true;
            return result;
        }
    }
}
