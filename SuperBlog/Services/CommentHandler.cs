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
    public class CommentHandler
    {
        private readonly IRepository<Comment> commentRepo;
        private readonly IRepository<Post> postRepo;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public CommentHandler(IRepository<Comment> commentRepository, IRepository<Post> postRepository, IMapper mapper, UserManager<User> userManager)
        {
            commentRepo = commentRepository;
            postRepo = postRepository;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public async Task<EditCommentViewModel> SetupEdit(Guid id, ClaimsPrincipal principal)
        {
            var comment = await commentRepo.GetByIdAsync(id) ?? throw new CommentNotFoundException();
            var userId = userManager.GetUserId(principal);
            if (comment.UserId.ToString() != userId && !principal.IsInRole("moderator")) throw new AccessDeniedException();
            var post = await postRepo.GetAll().Include(p => p.User).Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == comment.PostId)
                        ?? throw new PostNotFoundException();
            var postmodel = mapper.Map<PostViewModel>(post);
            var model = new EditCommentViewModel { Content = comment.Content, Id = comment.Id, Post = postmodel };
            return model;
        }

        public async Task<CommentHandlingResult> HandleEdit(EditCommentViewModel model, ClaimsPrincipal principal)
        {
            var result = new CommentHandlingResult();
            var comment = await commentRepo.GetByIdAsync(model.Id) ?? throw new CommentNotFoundException();
            var userId = userManager.GetUserId(principal);
            if (comment.UserId.ToString() != userId && !principal.IsInRole("moderator")) throw new AccessDeniedException();
            comment.Update(model);
            await commentRepo.UpdateAsync(comment);
            result.Success = true;
            result.PostId = comment.PostId;
            return result;
        }

        public async Task<WriteCommentViewModel> SetupWrite(Guid postId)
        {
            var post = await postRepo.GetAll().Include(p => p.User).FirstOrDefaultAsync(p => p.Id == postId)
                             ?? throw new PostNotFoundException();
            var postModel = mapper.Map<PostViewModel>(post);
            var model = new WriteCommentViewModel { Post = postModel };
            return model;
        }

        public async Task<CommentHandlingResult> HandleWrite(WriteCommentViewModel model, ClaimsPrincipal principal)
        {
            var result = new CommentHandlingResult();
            var post = await postRepo.GetByIdAsync(model.PostId) ?? throw new PostNotFoundException();
            var userId = userManager.GetUserId(principal) ?? throw new UserNotFoundException();
            var comment = new Comment
            {
                Content = model.Content,
                Post = post,
                UserId = new Guid(userId),
            };
            await commentRepo.AddAsync(comment);
            result.PostId = post.Id;
            result.Success = true;
            return result;
        }

        public async Task<CommentHandlingResult> HandleDelete(Guid id, ClaimsPrincipal principal)
        {
            var result = new CommentHandlingResult();
            var comment = await commentRepo.GetByIdAsync(id) ?? throw new CommentNotFoundException();
            var userId = userManager.GetUserId(principal) ?? throw new UserNotFoundException();
            if (comment.UserId.ToString() != userId && !principal.IsInRole("moderator")) throw new AccessDeniedException();
            result.PostId = comment.PostId;
            await commentRepo.DeleteAsync(comment);
            result.Success = true;
            return result;
        }
    }
}
