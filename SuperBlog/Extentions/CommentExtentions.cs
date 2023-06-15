using Microsoft.EntityFrameworkCore.Update.Internal;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;

namespace SuperBlog.Extentions
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
