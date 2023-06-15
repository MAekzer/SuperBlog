using System.ComponentModel.DataAnnotations;

namespace SuperBlog.Models.ViewModels
{
    public class EditCommentViewModel
    {
        public PostViewModel Post { get; set; }
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Комментарий не может быть пустым")]
        [MaxLength(1500, ErrorMessage = "Комментарий не может быть длиннее, чем 1500 символов")]
        public string Content { get; set; }
    }
}
