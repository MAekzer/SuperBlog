using System.ComponentModel.DataAnnotations;

namespace SuperBlog.Models.Entities
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
