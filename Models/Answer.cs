namespace CourseProject.Models
{
    public abstract class Answer
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public Form Form { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }

    public class TextAnswer : Answer
    {
        public string? TextAnswerValue { get; set; }
    }

    public class IntegerAnswer : Answer
    {
        public long? IntegerAnswerValue { get; set; }
    }

    public class CheckboxAnswer : Answer
    {
        public bool? CheckboxAnswerValue { get; set; }
    }
}
