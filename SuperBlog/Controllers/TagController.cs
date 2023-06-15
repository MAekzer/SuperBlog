using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Data.Repositories;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;

namespace SuperBlog.Controllers
{
    public class TagController : Controller
    {
        private readonly IRepository<Tag> tagRepo;
        private readonly IRepository<Post> postRepo;
        private readonly UserManager<User> userManager;

        public TagController(IRepository<Tag> tagRepository, IRepository<Post> postRepo, UserManager<User> userManager)
        {
            tagRepo = tagRepository;
            this.postRepo = postRepo;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AllTags()
        {
            var tags = await tagRepo.GetAll().Include(t => t.Posts).ToListAsync();
            var dict = new Dictionary<Tag, int>();
            foreach (var tag in tags)
            {
                dict.Add(tag, tag.Posts.Count);
            }
            var model = new TagViewModel { Tags = dict };
            return View("/Views/Tags/AllTags.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Tag(Guid id)
        {
            var user = await userManager.GetUserAsync(User);
            var tag = await tagRepo.GetAll().Include("Posts.User").Include("Posts.Tags").FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null) return View("/Views/Error/TagNotFound.cshtml");
            var model = new TagPostsViewModel { Name = tag.Name, User = user, Posts = tag.Posts };

            return View("/Views/Tags/TagPosts.cshtml", model);
        }

        [HttpPost]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Create(TagViewModel model)
        {
            var tag = new Tag { Name = model.Name };
            await tagRepo.AddAsync(tag);
            return RedirectToAction("AllTags", "Tag");
        }

        [HttpGet]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Update(Guid id)
        {
            var tag = await tagRepo.GetByIdAsync(id);
            if (tag == null) return View("/Views/Tags/TagNotFound.cshtml");
            var model = new EditTagViewModel { Id = id, Name = tag.Name };
            return View("/Views/Tags/EditTag.cshtml", model);
        }

        [HttpPost]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Update(EditTagViewModel model)
        {
            var tag = await tagRepo.GetByIdAsync(model.Id);
            if (tag == null) return View("/Views/Tags/TagNotFound.cshtml");

            tag.Name = model.Name;
            await tagRepo.UpdateAsync(tag);

            return RedirectToAction("AllTags", "Tag");
        }

        [HttpPost]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var tag = await tagRepo.GetByIdAsync(id);
            if (tag != null) await tagRepo.DeleteAsync(tag);

            var tags = await tagRepo.GetAll().ToListAsync();
            return RedirectToAction("AllTags", "Tag");
        }
    }
}
