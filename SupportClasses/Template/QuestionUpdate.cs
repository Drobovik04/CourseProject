using CourseProject.Attributes;
using CourseProject.Models;

namespace CourseProject.SupportClasses.Template
{
    public class QuestionUpdate
    {
        public int Id { get; set; }

        [LocalizedRequired(DisplayName = "QuestionTitle")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public QuestionType Type { get; set; }
        public bool ShowInResults { get; set; }
        public int Order { get; set; }
    }
}
