using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBlogData.Models.Responses
{
    public class CommentResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public bool IsRedacted { get; set; } = false;
        public DateTime? RedactionTime { get; set; } = null;
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
    }
}
