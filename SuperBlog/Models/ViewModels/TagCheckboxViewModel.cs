namespace SuperBlog.Models.ViewModels
{
    public class TagCheckboxViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; } = false;
    }
}
