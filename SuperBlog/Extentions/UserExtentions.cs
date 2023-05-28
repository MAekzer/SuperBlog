using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;

namespace SuperBlog.Extentions
{
    public static class UserExtentions
    {
        public static void Update(this User user, UpdateUserViewModel model)
        {
            if (!string.IsNullOrEmpty(model.UserName))
                user.UserName = model.UserName;
            if (!string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;
            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;
            if (!string.IsNullOrEmpty (model.LastName))
                user.LastName = model.LastName;
            if (!string.IsNullOrEmpty(model.MiddleName))
                user.MiddleName = model.MiddleName;
        }
    }
}
