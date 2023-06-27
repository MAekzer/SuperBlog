using Microsoft.AspNetCore.Identity;
using SuperBlogData.Models.ViewModels;

namespace SuperBlogData.Models.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public Role(string Name, string DisplayName, string Description) : base(Name) 
        {
            this.DisplayName = DisplayName;
            this.Description = Description;
        }

        public void Update(EditRoleViewModel model)
        {
            Name = model.Name;
            Description = model.Description;
            DisplayName = model.DisplayName;
        }
    }
}
