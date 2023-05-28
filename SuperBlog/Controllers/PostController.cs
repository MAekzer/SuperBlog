using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Data.Repositories;
using SuperBlog.Extentions;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;

namespace SuperBlog.Controllers
{
    public class PostController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IRepository<Post> postRepo;
        private readonly IRepository<Tag> tagRepo;

        public PostController(IRepository<Post> postRepo, UserManager<User> userManager, IMapper mapper, IRepository<Tag> tagRepo)
        {
            this.postRepo = postRepo;
            this.userManager = userManager;
            this.mapper = mapper;
            this.tagRepo = tagRepo;
        }

        [HttpGet]
        [Route("CreatePost")]
        public async Task<IActionResult> Create()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            var model = new PostViewModel { User = user };

            return View("/Views/Account/NewPost.cshtml", model);
        }

        [HttpPost]
        [Route("CreatePost")]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            model.User = user;
            if (!ModelState.IsValid) return View("/Views/Account/NewPost.cshtml", model);

            var newPost = mapper.Map<Post>(model);
            newPost.User = user;
            var tagNames = model.GetTags();

            foreach (var tagName in tagNames)
            {
                var tag = await tagRepo.GetByNameAsync(tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    await tagRepo.AddAsync(tag);
                }
                newPost.Tags.Add(tag);
            }

            await postRepo.AddAsync(newPost);

            return RedirectToAction("MyFeed", "User");
        }

        [HttpGet]
        [Route("UpdatePost")]
        public async Task<IActionResult> Update(string id)
        {
            var post = await postRepo.GetByIdAsync(id);

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.Id != post.UserId) return RedirectToAction("Index", "Home");

            var model = mapper.Map<PostViewModel>(post);
            return View("/Views/Account/EditPost.cshtml", model);
        }

        [HttpPost]
        [Route("UpdatePost")]
        public async Task<IActionResult> Update(PostViewModel model, string id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Index", "Home");

            var post = await postRepo.GetByIdAsync(id);
            if (currentUser.Id != post.UserId) return RedirectToAction("Index", "Home");

            post.Update(model);

            post.Tags = new List<Tag>();
            var newTags = model.GetTags();

            foreach (var tagName in newTags)
            {
                var tag = await tagRepo.GetByNameAsync(tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    await tagRepo.AddAsync(tag);
                }
                post.Tags.Add(tag);
            }

            await postRepo.UpdateAsync(post);
            return RedirectToAction("MyFeed", "User");
        }

        [HttpPost]
        [Route("DeletePost")]
        public async Task<IActionResult> Delete(string id)
        {
            var post = await postRepo.GetByIdAsync(id);
            if (post != null) await postRepo.DeleteAsync(post);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("AllPosts")]
        public async Task<IActionResult> Posts()
        {
            var posts = await postRepo.GetAll().ToListAsync();

            return View("/Views/Posts/AllPosts.cshtml");
        }

        [HttpGet]
        [Route("UserPosts")]
        public async Task<IActionResult> UserPosts(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return RedirectToAction("Index", "Home");

            var userPosts = postRepo.GetAll().Include(p => p.Comments).Where(p => p.UserId.Equals(id));
            var model = new UserPostsViewModel { User = user, Posts = await userPosts.ToListAsync() };

            return View("/Views/Posts/UserPosts.cshtml", userPosts);
        }

        [HttpGet]
        [Route("Post")]
        public async Task<IActionResult> Post(string id)
        {
            var post = await postRepo.GetByIdAsync(id);
            if (post == null) return View("Views/Posts/PostNotFound");

            return View("/Views/Posts/Post.cshtml", post);
        }
    }
}
