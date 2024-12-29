using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models
{
    public enum AccessType
    {
        Public,
        Restricted
    }
    public class Template
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? DescriptionWithMarkdown { get; set; }
        public int? TopicId { get; set; }
        public Topic? Topic { get; set; }
        public string? ImageUrl { get; set; }
        public AccessType AccessType { get; set; }
        public IdentityUser Author { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Form> Forms { get; set; } = new List<Form>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<TemplateAccess> AllowedUsers { get; set; } = new List<TemplateAccess>();
        public ICollection<TemplateTag> TemplateTags { get; set; } = new List<TemplateTag>();
        public DateTime CreatedAt { get; set; }
    }
}
