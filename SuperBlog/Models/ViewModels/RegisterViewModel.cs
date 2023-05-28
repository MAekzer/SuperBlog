using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SuperBlog.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
        [DisplayName("Имя")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Поле Фамилия обязательно для заполнения")]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }
        [DisplayName("Отчество (при наличии)")]
        public string? MiddleName { get; set; }
        [Required(ErrorMessage = "Поле Никнейм обязательно для заполнения")]
        [DisplayName("Никнейм")]
        [MinLength(3, ErrorMessage = "Никнейм не может быть чем 3 символа")]
        [MaxLength(20, ErrorMessage = "Никнейм не может быть чем 20 символов")]
        public string UserName { get; set; }
        [DisplayName("Пароль")]
        [Required(ErrorMessage = "Поле Пароль обязательно для заполнения")]
        [MinLength(3, ErrorMessage = "Пароль не может быть чем 3 символа")]
        [MaxLength(20, ErrorMessage = "Пароль не может быть чем 20 символов")]
        public string Password { get; set; }
        [DisplayName("Подтвердите пароль")]
        public string PasswordConfirm { get; set; }
        [DisplayName("Адрес электронной почты")]
        [Required(ErrorMessage = "Поле Email обязательно для заполнения")]
        public string Email { get; set; }
        [DisplayName("Дата рождения")]
        [Required(ErrorMessage = "Поле Дата рождения обязательно для заполнения")]
        public DateTime BirthDate { get; set; }

    }
}
