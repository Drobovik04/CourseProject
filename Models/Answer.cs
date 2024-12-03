namespace CourseProject.Models
{
    public abstract class Answer
    {
        public int Id { get; set; }
        public int FormAnswerId { get; set; }
        public FormAnswer FormAnswer { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }

    public class TextAnswer : Answer
    {
        public string TextAnswerValue { get; set; }
    }

    public class IntegerAnswer : Answer
    {
        public int IntegerAnswerValue { get; set; }
    }

    public class CheckboxAnswer : Answer
    {
        public bool CheckboxAnswerValue { get; set; }
    }
}
