using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SuperBlog.Models.ViewModels
{
    public class UpdateUserViewModel
    {
        [DisplayName("Имя")]
        public string? FirstName { get; set; }
        [DisplayName("Фамилия")]
        public string? LastName { get; set; }
        [DisplayName("Отчество (при наличии)")]
        public string? MiddleName { get; set; }
        [DisplayName("Никнейм")]
        [MinLength(3, ErrorMessage = "Никнейм не может быть чем 3 символа")]
        [MaxLength(20, ErrorMessage = "Никнейм не может быть чем 20 символов")]
        public string? UserName { get; set; }
        [DisplayName("Пароль")]
        [MinLength(3, ErrorMessage = "Пароль не может быть чем 3 символа")]
        [MaxLength(20, ErrorMessage = "Пароль не может быть чем 20 символов")]
        public string? Email { get; set; }
    }
}
