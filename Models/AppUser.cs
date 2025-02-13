using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models
{
    public class AppUser:IdentityUser
    {
        public string? SalesforceAccountId { get; set; }
        public string? SalesforceContactId { get; set; }
    }
}
