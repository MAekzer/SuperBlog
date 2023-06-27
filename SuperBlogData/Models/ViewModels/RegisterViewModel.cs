using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SuperBlogData.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
        [DisplayName("Имя")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Поле Фамилия обязательно для заполнения")]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }
        [DisplayName("Отчество")]
        public string? MiddleName { get; set; }
        [Required(ErrorMessage = "Поле Никнейм обязательно для заполнения")]
        [DisplayName("Никнейм")]
        [MinLength(3, ErrorMessage = "Никнейм не может короче быть чем 3 символа")]
        [MaxLength(20, ErrorMessage = "Никнейм не может короче быть чем 20 символов")]
        public string UserName { get; set; }
        [DisplayName("Пароль")]
        [Required(ErrorMessage = "Поле Пароль обязательно для заполнения")]
        [MinLength(3, ErrorMessage = "Пароль не может быть короче чем 3 символа")]
        [MaxLength(20, ErrorMessage = "Пароль не может быть длиннее чем 20 символов")]
        public string Password { get; set; }
        [DisplayName("Подтвердите пароль")]
        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
        [DisplayName("Адрес электронной почты")]
        [Required(ErrorMessage = "Поле Email обязательно для заполнения")]
        public string Email { get; set; }
        [DisplayName("День")]
        [Required(ErrorMessage = "Поле День обязательно для заполнения")]
        public string Day { get; set; }
        [DisplayName("Месяц")]
        [Required(ErrorMessage = "Поле Месяц обязательно для заполнения")]
        public string Month { get; set; }
        [DisplayName("Год")]
        [Required(ErrorMessage = "Поле Год обязательно для заполнения")]
        public string Year { get; set; }
        [DisplayName("О себе")]
        public string About { get; set; }

        public DateTime MakeBirthDate()
        {
            return new DateTime(Int32.Parse(Year), Int32.Parse(Month), Int32.Parse(Day));
        }

        public string MakeFullName()
        {
            if (string.IsNullOrEmpty(MiddleName))
                return $"{LastName} {FirstName}";
            return $"{LastName} {FirstName} {MiddleName}";
        }
    }
}
