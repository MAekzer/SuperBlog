using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperBlogData.Repositories;
using SuperBlogData.Exceptions;
using SuperBlogData.Extentions;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.ViewModels;
using SuperBlog.Services.Results;
using System.Security.Claims;

namespace SuperBlog.Services
{
    public class PostHandler
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IRepository<Post> postRepo;
        private readonly IRepository<Tag> tagRepo;

        public PostHandler(IMapper mapper,
            UserManager<User> userManager,
            IRepository<Post> postRepo,
            IRepository<Tag> tagRepo)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.postRepo = postRepo;
            this.tagRepo = tagRepo;
        }

        public async Task<CreatePostViewModel> SetupCreate(ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal) ?? throw new UserNotFoundException();
            var model = new CreatePostViewModel { UserId = user.Id };
            var tags = await tagRepo.GetAll().ToListAsync();

            foreach (var tag in tags)
            {
                var tagModel = mapper.Map<TagCheckboxViewModel>(tag);
                model.Tags.Add(tagModel);
            }

            return model;
        }

        public async Task<PostHandlingResult> HandleCreate(CreatePostViewModel model, ClaimsPrincipal principal)
        {
            var result = new PostHandlingResult();
            var user = await userManager.GetUserAsync(principal) ?? throw new UserNotFoundException();
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
            result.Success = true;
            return result;
        }

        public async Task<EditPostViewModel> SetupUpdate(Guid id, ClaimsPrincipal principal)
        {
            var post = await postRepo.GetByIdAsync(id);
            var currentUser = await userManager.GetUserAsync(principal);

            if (post == null) throw new PostNotFoundException();
            if (currentUser == null) throw new UserNotFoundException();
            if (currentUser.Id != post.UserId && !principal.IsInRole("moderator")) throw new AccessDeniedException();

            var model = mapper.Map<EditPostViewModel>(post);
            var tags = await tagRepo.GetAll().ToListAsync();

            foreach (var tag in tags)
            {
                var tagModel = new TagCheckboxViewModel { Id = tag.Id, Name = tag.Name, IsChecked = false };
                if (post.Tags.Contains(tag))
                {
                    tagModel.IsChecked = true;
                }
                model.Tags.Add(tagModel);
            }

            return model;
        }

        public async Task<PostHandlingResult> HandleUpdate(EditPostViewModel model, ClaimsPrincipal principal)
        {
            var result = new PostHandlingResult();
            var currentUser = await userManager.GetUserAsync(principal);
            var post = await postRepo.GetByIdAsync(model.Id);

            if (currentUser == null) throw new UserNotFoundException();
            if (post == null) throw new PostNotFoundException();
            if (currentUser.Id != post.UserId && !principal.IsInRole("moderator")) throw new AccessDeniedException();

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
            result.Success = true;
            return result;
        }

        public async Task<PostHandlingResult> HandleDelete(Guid id, ClaimsPrincipal principal)
        {
            var result = new PostHandlingResult();
            var userId = userManager.GetUserId(principal);
            var post = await postRepo.GetByIdAsync(id);
            if (string.IsNullOrEmpty(userId)) throw new UserNotFoundException();
            if (post is null) throw new PostNotFoundException();
            if (userId != post.UserId.ToString() && !principal.IsInRole("moderator")) throw new AccessDeniedException();
            await postRepo.DeleteAsync(post);
            result.Success = true;
            return result;
        }

        public async Task<PostsViewModel> SetupPosts(ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            var posts = await postRepo.GetAll().Include(p => p.User).Include(p => p.Tags).ToListAsync();
            var tags = tagRepo.GetAll();

            var model = new PostsViewModel { Posts = posts, User = user };
            foreach (var tag in tags)
            {
                var tagModel = new TagCheckboxViewModel { Id = tag.Id, Name = tag.Name, IsChecked = false };
                model.Tags.Add(tagModel);
            }
            return model;
        }

        public async Task<PostsViewModel> SetupPosts(PostsViewModel model, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            var posts = await postRepo.GetAll().Include(p => p.User).Include(p => p.Tags).ToListAsync();

            if (!string.IsNullOrEmpty(model.SearchPrompt))
                posts = postRepo.GetAll().Where(p => p.Title.Contains(model.SearchPrompt)).ToList();

            if (model.Tags.Count > 0)
            {
                var tags = new List<Tag>();
                foreach (var tagModel in model.Tags)
                {
                    if (tagModel.IsChecked)
                    {
                        posts = posts.Where(p => p.Tags.Any(t => t.Id == tagModel.Id)).ToList();
                    }
                }
            }
            model.Posts = posts;
            model.User = user;
            return model;
        }

        public async Task<PostViewModel> SetupPost(Guid id, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            var post = await postRepo.GetAll().Include(p => p.User).Include(p => p.Tags)
                                     .Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id)
                                     ?? throw new PostNotFoundException();

            var model = mapper.Map<PostViewModel>(post);
            if (user != null) model.User = user;
            model.Comments = post.Comments;
            return model;
        }
    }
}
