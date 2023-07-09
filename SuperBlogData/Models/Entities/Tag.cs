using SuperBlogData.Models.Requests;
using System.ComponentModel.DataAnnotations;

namespace SuperBlogData.Models.Entities
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();

        public void Update(TagRequest request)
        {
            Name = request.Name;
        }
    }
}
