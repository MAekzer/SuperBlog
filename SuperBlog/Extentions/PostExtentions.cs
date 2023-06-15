using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;
using System.Text;
using System.Text.RegularExpressions;

namespace SuperBlog.Extentions
{
    public static class PostExtentions
    {
        public static void Update(this Post post, EditPostViewModel model)
        {
            post.Title = model.Title;
            post.Content = model.Content;
            post.IsRedated = true;
            post.RedactionTime = DateTime.Now;
        }

        public static DateTime? GetTime(this Post post)
        {
            if (post.IsRedated)
                return post.RedactionTime;
            else
                return post.CreationTime;
        }
    }
}
