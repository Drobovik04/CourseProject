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
        public string? Description { get; set; }
        public QuestionType Type { get; set; }
        public bool ShowInResults { get; set; }
        public int TemplateId { get; set; }
        public Template Template { get; set; }
        public int Order { get; set; }
    }
}
