namespace CourseProject.Models
{
    public enum QuestionType
    {
        SingleLineText,
        MultiLineText,
        Integer,
        Checkbox
    }
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public QuestionType Type { get; set; }
        public bool ShowInResults { get; set; }
        public int FormId { get; set; }
        public Form Form { get; set; }
        public int Order {  get; set; }
    }
}
