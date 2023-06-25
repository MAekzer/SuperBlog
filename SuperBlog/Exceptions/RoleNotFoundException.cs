namespace SuperBlog.Exceptions
{
    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException() : base() { }
        public RoleNotFoundException(string message) : base(message) { } 
    }
}
