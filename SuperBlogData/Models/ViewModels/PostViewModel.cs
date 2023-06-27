using SuperBlogData.Models.Entities;

namespace SuperBlogData.Models.ViewModels
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsRedacted { get; set; }
        public DateTime? RedactionTime { get; set; }
        public User User { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
