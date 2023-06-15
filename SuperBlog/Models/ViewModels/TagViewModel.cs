using SuperBlog.Models.Entities;

namespace SuperBlog.Models.ViewModels
{
    public class TagViewModel
    {
        public string Name { get; set; }
        public Dictionary<Tag, int> Tags { get; set; } = new Dictionary<Tag, int>();
    }
}
