using CourseProject.Models;

namespace CourseProject.ViewModels.Template
{
    public class TemplateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public AccessType AccessType { get; set; }
        public List<string>? Tags { get; set; } = new();
        public List<string>? AllowedUserNames { get; set; } = new();
        public List<QuestionViewModel> Questions { get; set; } = new();
        public int? TopicId { get; set; }
        public List<Topic> Topics { get; set; } = new();
    }
}
