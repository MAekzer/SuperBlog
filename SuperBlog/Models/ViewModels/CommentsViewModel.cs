using SuperBlog.Models.Entities;

namespace SuperBlog.Models.ViewModels
{
    public class CommentsViewModel
    {
        public Post Post { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
