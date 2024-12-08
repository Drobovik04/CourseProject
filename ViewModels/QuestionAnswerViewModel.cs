using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class QuestionAnswerViewModel
    {
        public int QuestionId { get; set; }
        public string Title { get; set; }
        public QuestionType Type { get; set; }
    }
}
