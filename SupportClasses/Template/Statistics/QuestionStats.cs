using CourseProject.Models;

namespace CourseProject.SupportClasses.Template.Statistics
{
    public class QuestionStats
    {
        public string QuestionTitle { get; set; }
        public string QuestionDescription { get; set; }
        public QuestionType QuestionType { get; set; }

        public IntegerStatistics? IntegerStats { get; set; }

        public TextStatistics? TextStats { get; set; }

        public CheckboxStatistics? CheckboxStats { get; set; }
    }
}
