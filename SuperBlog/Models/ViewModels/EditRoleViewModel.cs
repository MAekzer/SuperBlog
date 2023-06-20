using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SuperBlog.Models.ViewModels
{
    public class EditRoleViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Название роли не может быть пустым")]
        [MaxLength(255, ErrorMessage = "Максимальная длина названия роли - 255 символов")]
        [DisplayName("Системное название")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Название роли не может быть пустым")]
        [MaxLength(255, ErrorMessage = "Максимальная длина названия роли - 255 символов")]
        [DisplayName("Название для отображения")]
        public string DisplayName { get; set; }
        [Required(ErrorMessage = "Описание роли не может быть пустым")]
        [MaxLength(255, ErrorMessage = "Максимальная длина описания роли - 255 символов")]
        [DisplayName("Описание")]
        public string Description { get; set; }
    }
}
