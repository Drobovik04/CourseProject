using CourseProject.Models;

namespace CourseProject.SupportClasses.Form
{
    public class QuestionAnswerPair
    {
        public Question Question { get; set; }
        public Answer? Answer { get; set; }
    }
}
