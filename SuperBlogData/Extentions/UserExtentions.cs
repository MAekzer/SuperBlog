using SuperBlogData.Models.Entities;
using SuperBlogData.Models.Requests;
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
        
        public static void Update(this User user, UserPutRequest request)
        {
            user.UserName = request.UserName;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.MiddleName = request.MiddleName;
            user.BirthDate = request.BirthDate;
            user.About = request.About;

            var fullname = user.GetFullName();
            user.FullName = fullname;
            user.NormalizedFullName = fullname.ToUpper();
        }
    }
}
