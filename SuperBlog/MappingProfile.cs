using AutoMapper;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.ViewModels;

namespace SuperBlog
{
    public class MappingProfile : Profile
    {
        public MappingProfile() : base()
        {
            CreateMap<RegisterViewModel, User>()
                .ForMember(u => u.BirthDate, opt => opt.MapFrom(m => m.MakeBirthDate()))
                .ForMember(u => u.FullName, opt => opt.MapFrom(m => m.MakeFullName()))
                .ForMember(u => u.NormalizedFullName, opt => opt.MapFrom(m => m.MakeFullName().ToUpper()));
            CreateMap<CreatePostViewModel, Post>()
                .ForMember(p => p.Tags, opt => opt.MapFrom(m => new List<Tag>()))
                .ForMember(p => p.Id, opt => opt.MapFrom(m => Guid.NewGuid()))
                .ForMember(p => p.CreationTime, opt => opt.MapFrom(m => DateTime.Now));
            CreateMap<Post, EditPostViewModel>()
                .ForMember(m => m.Tags, opt => opt.MapFrom(p => new List<TagCheckboxViewModel>()));
            CreateMap<Post, PostViewModel>();
            CreateMap<Comment, WriteCommentViewModel>();
            CreateMap<Tag, TagCheckboxViewModel>();
            CreateMap<User, EditUserViewModel>()
                .ForMember(m => m.Day, opt => opt.MapFrom(u => u.BirthDate.Day))
                .ForMember(m => m.Month, opt => opt.MapFrom(u => u.BirthDate.Month))
                .ForMember(m => m.Year, opt => opt.MapFrom(u => u.BirthDate.Year));
        }
    }
}
