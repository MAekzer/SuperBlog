using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SuperBlog.Models.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("Логин")]
        [Required(ErrorMessage = "Введите Логин")]
        public string UserName { get; set; }
        [DisplayName("Пароль")]
        [Required(ErrorMessage = "Введите Пароль")]
        public string Password { get; set; }
    }
}
