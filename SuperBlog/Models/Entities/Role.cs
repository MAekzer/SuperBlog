using Microsoft.AspNetCore.Identity;

namespace SuperBlog.Models.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public List<User> Users { get; set; } = new List<User>();

        public Role(string name) : base(name) { }
    }
}
