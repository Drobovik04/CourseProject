using CourseProject.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.ViewModels.Template
{
    public class TemplateUpdateViewModel
    {
        public int Id { get; set; }
        public IdentityUser Author { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? CurrentImageUrl { get; set; }
        public IFormFile? NewImageFile { get; set; }
        public AccessType AccessType { get; set; }
        public List<string>? Tags { get; set; } = new();
        public List<string>? AllowedUserNames { get; set; } = new();
        public List<QuestionUpdateViewModel> Questions { get; set; } = new();
        public int? TopicId { get; set; }
        public List<Topic> Topics { get; set; } = new();
    }
}
