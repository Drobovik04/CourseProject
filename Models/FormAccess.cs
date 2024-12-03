using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models
{
    public class FormAccess
    {
        public int FormId { get; set; }
        public Form Form { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
