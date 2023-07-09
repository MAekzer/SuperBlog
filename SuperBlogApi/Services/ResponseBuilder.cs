using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperBlogData;
using SuperBlogData.Extentions;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.Responses;
using SuperBlogData.Repositories;

namespace SuperBlogApi.Services
{
    public class ResponseBuilder
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly BlogContext db;

        public ResponseBuilder(IMapper mapper, UserManager<User> userManager, BlogContext db, RoleManager<Role> roleManager)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.db = db;
            this.roleManager = roleManager;
        }

        public async Task<UserResponse> BuildUserResponse(User user)
        {
            if (user == null) return new UserResponse();

            var response = mapper.Map<UserResponse>(user);
            response.Roles = await roleManager.GetRoleIds(user, userManager);
            response.Posts = await db.Posts.AsQueryable().Where(p => p.UserId == user.Id).Select(p => p.Id).ToListAsync();
            response.Comments = await db.Comments.AsQueryable().Where(c => c.UserId == user.Id).Select(p => p.Id).ToListAsync();

            return response;
        }

        public async Task<PostResponse> BuildPostResponse(Post post)
        {
            if (post == null) return new PostResponse();

            var response = mapper.Map<PostResponse>(post);
            response.Tags = await db.Tags.AsQueryable().Where(t => t.Posts.Any(p => p.Id == post.Id)).Select(t => t.Id).ToListAsync();
            response.Comments = await db.Comments.AsQueryable().Where(c => c.PostId == post.Id).Select(c => c.Id).ToListAsync();
            return response;
        }

        public async Task<TagResponse> BuildTagResponse(Tag tag)
        {
            if (tag == null) return new TagResponse();

            var response = mapper.Map<TagResponse>(tag);
            response.Posts = await db.Posts.AsQueryable().Where(p => p.Tags.Any(t => t.Id == tag.Id)).Select(p => p.Id).ToListAsync();
            return response;
        }

        public CommentResponse BuildCommentResponse(Comment comment)
        {
            if (comment == null) return new CommentResponse();

            var response = mapper.Map<CommentResponse>(comment);
            return response;
        }
    }
}
