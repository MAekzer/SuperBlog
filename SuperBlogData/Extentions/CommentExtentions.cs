using Microsoft.EntityFrameworkCore.Update.Internal;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.ViewModels;

namespace SuperBlogData.Extentions
{
    public static class CommentExtentions
    {
        public static void Update(this Comment comment, EditCommentViewModel model)
        {
            if (model == null) return;
            comment.IsRedacted = true;
            comment.RedactionTime = DateTime.Now;
            comment.Content = model.Content;
        }
    }
}
