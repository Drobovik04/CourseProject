using CourseProject.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models
{
    public class Form
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public Template Template { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public DateTime SubmittedAt { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }

}