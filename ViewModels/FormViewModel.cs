using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class FormViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public AccessType AccessType { get; set; }
        public List<string>? Tags { get; set; } = new();
        public List<string>? AllowedUserNames { get; set; } = new();
        public List<QuestionViewModel> Questions { get; set; } = new();
    }
}
