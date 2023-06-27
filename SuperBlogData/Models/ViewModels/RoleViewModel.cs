using SuperBlogData.Models.Entities;

namespace SuperBlogData.Models.ViewModels
{
    public class RoleViewModel
    {
        public Role Role { get; set; }
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }
}
