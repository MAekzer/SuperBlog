using SuperBlogData.Models.Entities;
using SuperBlogData.Models.ViewModels;

namespace SuperBlogData.Extentions
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

            var fullname = user.GetFullName();
            user.FullName = fullname;
            user.NormalizedFullName = fullname.ToUpper();
        }
        
    }
}
