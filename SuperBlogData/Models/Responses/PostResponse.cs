using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBlogData.Models.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsRedated { get; set; }
        public DateTime? RedactionTime { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> Tags { get; set; } = new List<Guid>();
        public List<Guid> Comments { get; set; } = new List<Guid>();
    }
}
