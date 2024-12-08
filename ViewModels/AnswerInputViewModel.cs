using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class AnswerInputViewModel
    {
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public QuestionType Type { get; set; }
    }
}
