using SuperBlog.Models.Entities;
using System.Text;

namespace SuperBlog.Models.ViewModels
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsRedated { get; set; }
        public DateTime? RedactionTime { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Tag> Tags { get; set; } = new List<Tag>();

        public List<string> GetTags()
        {
            List<string> tags = new();
            StringBuilder sb = new();

            for (int i = 0; i < Content.Length; i++)
            {
                if (Content[i] == '#')
                {
                    for (int j = i + 1; j < Content.Length; j++)
                    {
                        if (Char.IsLetterOrDigit(Content[j]))
                            sb.Append(Content[j]);
                        else
                            break;
                    }
                    tags.Add(sb.ToString());
                    sb.Clear();
                }
            }
            return tags;
        }
    }
}
