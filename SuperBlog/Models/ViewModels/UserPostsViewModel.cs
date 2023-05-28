using SuperBlog.Models.Entities;

namespace SuperBlog.Models.ViewModels
{
    public class UserPostsViewModel
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
