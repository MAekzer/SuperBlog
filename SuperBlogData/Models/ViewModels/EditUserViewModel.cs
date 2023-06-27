using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using SuperBlogData.Models.Entities;

namespace SuperBlogData.Models.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public Guid Id { get; set; }
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
        public List<RoleCheckboxViewModel> Roles { get; set; } = new List<RoleCheckboxViewModel>();

        public DateTime MakeBirthDate()
        {
            return new DateTime(Int32.Parse(Year), Int32.Parse(Month), Int32.Parse(Day));
        }
    }

    public class RoleCheckboxViewModel
    {
        public Guid Id { get; set; }
        public string DispayName { get; set; }
        public bool IsChecked { get; set; }
    }
}
