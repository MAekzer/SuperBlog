using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace SuperBlogData.Models.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; set; }
        public string NormalizedFullName { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public DateTime BirthDate { get; set; }
        public string About { get; set; }
        public List<Guid> Roles { get; set; } = new List<Guid>();
        public List<Guid> Posts { get; set; } = new List<Guid>();
        public List<Guid> Comments { get; set; } = new List<Guid>();
    }
}
