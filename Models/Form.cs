using Microsoft.AspNetCore.Identity;
using Microsoft.SqlServer.Server;

namespace CourseProject.Models
{
    public enum AccessType
    {
        Public,
        Restricted
    }
    public class Form
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public AccessType AccessType { get; set; } // Enum: Public, Restricted
        public IdentityUser Author { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<FormAnswer> FormAnswers { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<FormAccess> AllowedUsers { get; set; }
        public ICollection<FormTag> FormTags { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
