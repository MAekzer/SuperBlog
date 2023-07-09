using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBlogData.Models.Responses
{
    public class TagResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> Posts { get; set; }
    }
}
