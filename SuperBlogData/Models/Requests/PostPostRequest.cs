using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBlogData.Models.Requests
{
    public class PostPostRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        [MinLength(50)]
        [MaxLength(10000)]
        public string Content { get; set; }
        [Required]
        public string UserId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
