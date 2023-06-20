namespace SuperBlog.Services
{
    public class UserHandlingResult
    {
        public bool Success { get; set; }
        public bool IncorrectLoginOrPassword { get; set; }
        public bool UserAlreadyExists { get; set; }
    }
}
