using Microsoft.AspNetCore.Identity;

namespace SuperBlog.Models.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? About { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Role> Roles { get; set; } = new List<Role>();
    }
}
