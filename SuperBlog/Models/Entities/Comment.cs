using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperBlog.Models.Entities
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public bool IsRedacted { get; set; } = false;
        public DateTime? RedactionTime { get; set; } = null;
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; }
    }
}