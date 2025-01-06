using CourseProject.Attributes;
using CourseProject.Models;
using System.ComponentModel;

namespace CourseProject.SupportClasses.Template
{
    public class QuestionCreate
    {
        [LocalizedRequired(DisplayName = "QuestionTitle")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public QuestionType Type { get; set; }
        public bool ShowInResults { get; set; }
    }
}
