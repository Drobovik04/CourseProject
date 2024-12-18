using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class QuestionViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public bool ShowInResults { get; set; }
    }
}
