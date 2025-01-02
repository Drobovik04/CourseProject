using CourseProject.Models;
using CourseProject.SupportClasses.Template.Statistics;

namespace CourseProject.ViewModels.Template
{
    public class TemplateStatisticsViewModel
    {
        public string QuestionTitle { get; set; }
        public string QuestionDescription { get; set; }
        public QuestionType QuestionType { get; set; }

        public IntegerStatistics? IntegerStats { get; set; }

        public TextStatistics? TextStats { get; set; }

        public CheckboxStatistics? CheckboxStats { get; set; }
    }
}
