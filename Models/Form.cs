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
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public AccessType AccessType { get; set; }
        public IdentityUser Author { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<FormAnswer> FormAnswers { get; set; } = new List<FormAnswer>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<FormAccess> AllowedUsers { get; set; } = new List<FormAccess>();
        public ICollection<FormTag> FormTags { get; set; } = new List<FormTag>();
        public DateTime CreatedAt { get; set; }
    }
}
