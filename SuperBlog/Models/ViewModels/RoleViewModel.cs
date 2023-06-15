using SuperBlog.Models.Entities;

namespace SuperBlog.Models.ViewModels
{
    public class RoleViewModel
    {
        public Role Role { get; set; }
        public List<User> Users { get; set; }
    }
}
