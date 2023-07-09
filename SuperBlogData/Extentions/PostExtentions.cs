using SuperBlogData.Models.Entities;
using SuperBlogData.Models.Requests;
using SuperBlogData.Models.ViewModels;
using System.Text;
using System.Text.RegularExpressions;

namespace SuperBlogData.Extentions
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

        public static void Update(this Post post, PostPutRequest request)
        {
            post.Title = request.Title;
            post.Content = request.Content;
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
