using CourseProject.Models;

namespace CourseProject.SupportClasses.Form
{
    public class QuestionAnswer
    {
        public int QuestionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
    }
}
