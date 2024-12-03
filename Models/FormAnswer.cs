using CourseProject.Models;

namespace CourseProject.Models
{
    public abstract class FormAnswer
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public Form Form { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
    public class TextFormAnswer : FormAnswer
    {
        public string TextAnswer { get; set; }
    }

    public class IntegerFormAnswer : FormAnswer
    {
        public int IntegerAnswer { get; set; }
    }

    public class CheckboxFormAnswer : FormAnswer
    {
        public bool CheckboxAnswer { get; set; }
    }
}