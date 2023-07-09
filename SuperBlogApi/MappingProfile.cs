using AutoMapper;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.Requests;
using SuperBlogData.Models.Responses;

namespace SuperBlogApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile() : base()
        {
            CreateMap<UserPostRequest, User>()
                .ForMember(u => u.FullName, opt => opt.MapFrom(r => r.MakeFullName()))
                .ForMember(u => u.NormalizedFullName, opt => opt.MapFrom(r => r.MakeFullName().ToUpper()));
            CreateMap<PostPostRequest, Post>()
                .ForMember(p => p.CreationTime, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(p => p.Tags, opt => opt.MapFrom(_ => new List<Tag>()));
            CreateMap<User, UserResponse>()
                .ForMember(r => r.Comments, opt => opt.MapFrom(_ => new List<Guid>()))
                .ForMember(r => r.Roles, opt => opt.MapFrom(_ => new List<Guid>()))
                .ForMember(r => r.Posts, opt => opt.MapFrom(_ => new List<Guid>()));
            CreateMap<Post, PostResponse>()
                .ForMember(r => r.Comments, opt => opt.MapFrom(_ => new List<Guid>()))
                .ForMember(r => r.Tags, opt => opt.MapFrom(_ => new List<Guid>()));
            CreateMap<Role, RoleResponse>();
            CreateMap<RolePostRequest, Role>();
            CreateMap<Tag, TagResponse>()
                .ForMember(r => r.Posts, opt => opt.MapFrom(_ => new List<Guid>()));
            CreateMap<TagRequest, Tag>();
            CreateMap<Tag, TagResponse>();
            CreateMap<CommentPostRequest, Comment>()
                .ForMember(c => c.RedactionTime, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(c => c.IsRedacted, opt => opt.MapFrom(_ => false));
            CreateMap<Comment, CommentResponse>();
        }
    }
}
