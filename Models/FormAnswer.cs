using CourseProject.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models
{
    public class FormAnswer
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public Form Form { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public DateTime SubmittedAt { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }

}