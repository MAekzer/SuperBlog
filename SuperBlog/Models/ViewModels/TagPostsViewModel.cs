using SuperBlog.Models.Entities;

namespace SuperBlog.Models.ViewModels
{
    public class TagPostsViewModel
    {
        public string Name { get; set; }
        public User User { get; set; }
        public List<Post> Posts { get; set; }
    }
}
