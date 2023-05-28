using AutoMapper;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;

namespace SuperBlog
{
    public class MappingProfile : Profile
    {
        public MappingProfile() : base()
        {
            CreateMap<RegisterViewModel, User>();
            CreateMap<UpdateUserViewModel, User>();
            CreateMap<PostViewModel, Post>();
            CreateMap<Post, PostViewModel>();
            CreateMap<Comment, WriteCommentViewModel>();
        }
    }
}
