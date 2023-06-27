using SuperBlogData.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SuperBlogData.Models.ViewModels
{
    public class CreatePostViewModel
    {
        [Required(ErrorMessage = "У статьи должно быть название")]
        [MaxLength(100, ErrorMessage = "Заголовок не может быть длиннее чем 100 символов")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Статья не может быть пустая")]
        [MinLength(50, ErrorMessage = "Минимальное количество символов в статье - 50")]
        [MaxLength(10000, ErrorMessage = "Максимальное количество символов в статье - 10000")]
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public List<TagCheckboxViewModel> Tags { get; set; } = new List<TagCheckboxViewModel>();
    }
}
