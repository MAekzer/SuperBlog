using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBlogData.Models.Requests
{
    public class CommentPostRequest
    {
        [Required]
        [MaxLength(10000)]
        public string Content { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string PostId { get; set; }
    }
}
