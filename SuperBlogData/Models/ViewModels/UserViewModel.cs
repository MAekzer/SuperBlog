using SuperBlogData.Models.Entities;

namespace SuperBlogData.Models.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Role> Roles { get; set; } = new List<Role>();
        public int PostCount { get; set; }
    }
}
