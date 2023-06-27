using SuperBlogData.Models.Entities;

namespace SuperBlogData.Models.ViewModels
{
    public class UserPostsViewModel
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
