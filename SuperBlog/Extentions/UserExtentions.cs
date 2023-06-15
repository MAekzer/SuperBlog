using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;

namespace SuperBlog.Extentions
{
    public static class UserExtentions
    {
        public static void Update(this User user, EditUserViewModel model)
        {
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MiddleName = model.MiddleName;
            user.BirthDate = model.MakeBirthDate();
            user.About = model.About;
        }

        public static string GetFullName(this User user)
        {
            if (string.IsNullOrEmpty(user.MiddleName))
                return $"{user.LastName} {user.FirstName}";
            return $"{user.LastName} {user.FirstName} {user.MiddleName}";
        }
    }
}
