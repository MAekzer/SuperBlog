using System.ComponentModel.DataAnnotations;

namespace SuperBlogData.Models.Entities
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public bool IsRedated { get; set; } = false;
        public DateTime? RedactionTime { get; set; } = null;
        public Guid UserId { get; set; }
        public User User { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}