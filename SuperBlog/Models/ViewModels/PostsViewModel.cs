using SuperBlog.Models.Entities;

namespace SuperBlog.Models.ViewModels
{
    public class PostsViewModel
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; }
        public List<TagCheckboxViewModel> Tags { get; set; } = new List<TagCheckboxViewModel>();
        public string SearchPrompt { get; set; }
    }
}
