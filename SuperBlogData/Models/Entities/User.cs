using Microsoft.AspNetCore.Identity;

namespace SuperBlogData.Models.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? FullName { get; set; }
        public string NormalizedFullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? About { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public string GetFullName()
        {
            if (string.IsNullOrEmpty(MiddleName))
                return $"{LastName} {FirstName}";
            return $"{LastName} {FirstName} {MiddleName}";
        }

        public User() : base()
        {
            string fullname = GetFullName();
            FullName = fullname;
            NormalizedFullName = fullname.ToUpper();
        }
    }
}
