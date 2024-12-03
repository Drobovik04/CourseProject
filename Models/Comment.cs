using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public Template Template { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
