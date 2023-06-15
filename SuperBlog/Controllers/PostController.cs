using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Data.Repositories;
using SuperBlog.Extentions;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;
using System.Xml.Linq;

namespace SuperBlog.Controllers
{
    public class PostController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IRepository<Post> postRepo;
        private readonly IRepository<Tag> tagRepo;
        private readonly IRepository<Comment> commentRepo;

        public PostController(
            IRepository<Post> postRepo,
            UserManager<User> userManager,
            IMapper mapper,
            IRepository<Tag> tagRepo,
            IRepository<Comment> commentRepo)
        {
            this.postRepo = postRepo;
            this.userManager = userManager;
            this.mapper = mapper;
            this.tagRepo = tagRepo;
            this.commentRepo = commentRepo;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            var model = new CreatePostViewModel { UserId = user.Id };
            var tags = await tagRepo.GetAll().ToListAsync();

            foreach (var tag in tags)
            {
                var tagModel = mapper.Map<TagCheckboxViewModel>(tag);
                model.Tags.Add(tagModel);
            }

            return View("/Views/Posts/NewPost.cshtml", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            model.UserId = user.Id;

            var newPost = mapper.Map<Post>(model);
            foreach (var tagModel in model.Tags)
            {
                if (tagModel.IsChecked)
                {
                    var tag = await tagRepo.GetByIdAsync(tagModel.Id);
                    newPost.Tags.Add(tag);
                }
            }

            await postRepo.AddAsync(newPost);

            return RedirectToAction("MyFeed", "User");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var post = await postRepo.GetByIdAsync(id);

            var currentUser = await userManager.GetUserAsync(User);
            if (post == null || currentUser == null) return RedirectToAction("Index", "Home");
            if (currentUser.Id != post.UserId && User.IsInRole("moderator")) return RedirectToAction("MyFeed", "User");

            var model = mapper.Map<EditPostViewModel>(post);
            var tags = await tagRepo.GetAll().ToListAsync();

            foreach (var tag in tags)
            {
                var tagModel = new TagCheckboxViewModel { Id = tag.Id, Name = tag.Name, IsChecked = false};
                if (post.Tags.Contains(tag))
                {
                    tagModel.IsChecked = true;
                }
                model.Tags.Add(tagModel);
            }

            return View("/Views/Posts/EditPost.cshtml", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(EditPostViewModel model)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var post = await postRepo.GetByIdAsync(model.Id);

            if (currentUser == null || post == null) return RedirectToAction("Index", "Home");
            if (currentUser.Id != post.UserId && !User.IsInRole("moderator")) return RedirectToAction("MyFeed", "User");

            post.Update(model);

            post.Tags = new List<Tag>();
            foreach (var tagModel in model.Tags)
            {
                if (tagModel.IsChecked)
                {
                    var tag = await tagRepo.GetByIdAsync(tagModel.Id);
                    post.Tags.Add(tag);
                }
            }

            await postRepo.UpdateAsync(post);
            return RedirectToAction("MyFeed", "User");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var post = await postRepo.GetByIdAsync(id);
            if (post != null) await postRepo.DeleteAsync(post);

            return RedirectToAction("MyFeed", "User");
        }

        [HttpGet]
        public async Task<IActionResult> Posts()
        {
            var user = await userManager.GetUserAsync(User);
            var posts = await postRepo.GetAll().Include(p => p.User).Include(p => p.Tags).ToListAsync();
            var tags = tagRepo.GetAll();

            var model = new PostsViewModel { Posts = posts, User = user };
            foreach (var tag in tags)
            {
                var tagModel = new TagCheckboxViewModel { Id = tag.Id, Name = tag.Name, IsChecked = false };
                model.Tags.Add(tagModel);
            }

            return View("/Views/Posts/AllPosts.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Posts(PostsViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            IQueryable<Post> posts = postRepo.GetAll().Include(p => p.User).Include(p => p.Tags);

            if (!string.IsNullOrEmpty(model.SearchPrompt))
                posts = postRepo.GetAll().Where(p => p.Title.Contains(model.SearchPrompt));

            if (model.Tags.Count > 0)
            {
                var tags = new List<Tag>();
                foreach (var tagModel in model.Tags)
                {
                    if (tagModel.IsChecked)
                    {
                        posts = posts.Where(p => p.Tags.Any(t => t.Id == tagModel.Id));
                    }
                }
            }
            model.Posts = await posts.ToListAsync();
            model.User = user;
            return View("/Views/Posts/AllPosts.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> UserPosts(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return RedirectToAction("Index", "Home");

            var userPosts = await postRepo.GetAll().Include(p => p.Comments).Where(p => p.UserId.Equals(id)).ToListAsync();
            var model = new UserPostsViewModel { User = user, Posts = userPosts };

            return View("/Views/Posts/UserPosts.cshtml", userPosts);
        }

        [HttpGet]
        public async Task<IActionResult> Post(Guid id)
        {
            var user = await userManager.GetUserAsync(User);
            var post = await postRepo.GetAll().Include(p => p.User).Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
            var comments = await commentRepo.GetAll().Include(c => c.User).ToListAsync();
            if (post == null) return View("Views/Error/PostNotFound.cshtml");

            var model = mapper.Map<PostViewModel>(post);
            if (user != null)  model.User = user;
            model.Comments = comments;

            return View("/Views/Posts/Post.cshtml", model);
        }
    }
}
