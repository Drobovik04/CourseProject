using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models
{
    public class TemplateAccess
    {
        public int TemplateId { get; set; }
        public Template Template { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
