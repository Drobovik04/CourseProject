using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class FormUpdateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public AccessType AccessType { get; set; }
        public List<QuestionUpdateViewModel> Questions { get; set; } = new();
    }
}
