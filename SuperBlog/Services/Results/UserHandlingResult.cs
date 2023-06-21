namespace SuperBlog.Services.Results
{
    public class UserHandlingResult
    {
        public bool Success { get; set; }
        public bool IncorrectLoginOrPassword { get; set; }
        public bool EmailAlreadyExists { get; set; }
        public bool LoginAlreadyExists { get; set; }
        public bool AccessDenied { get; set; }
    }
}
