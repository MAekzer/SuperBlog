namespace SuperBlog.Models.ViewModels
{
    public class UsersViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public string SearchParam { get; set; }
        public SearchCriteria? SearchCriterion { get; set; }
    }

    public enum SearchCriteria
    {
        Имя = 0,
        Email = 1,
        Логин = 2,
        End = 3
    }
}
