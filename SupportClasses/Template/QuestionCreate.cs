using CourseProject.Models;

namespace CourseProject.SupportClasses.Template
{
    public class QuestionCreate
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public bool ShowInResults { get; set; }
    }
}
