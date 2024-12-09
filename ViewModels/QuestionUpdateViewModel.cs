using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class QuestionUpdateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public QuestionType Type { get; set; }
        public bool ShowInResults { get; set; }
        public int Order { get; set; }
    }
}
