using CourseProject.Models;

namespace CourseProject.SupportClasses.Form
{
    public class AnswerInput
    {
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public QuestionType Type { get; set; }
    }
}
