using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class QuestionStatisticsViewModel
    {
        public string QuestionTitle { get; set; } 
        public QuestionType QuestionType { get; set; }

        public IntegerStatistics? IntegerStats { get; set; }

        public TextStatistics? TextStats { get; set; }

        public CheckboxStatistics? CheckboxStats { get; set; }
    }

    public class IntegerStatistics
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Mean { get; set; }
        public double Median { get; set; }
        public double StdDev { get; set; }
    }

    public class TextStatistics
    {
        public string MostFrequent { get; set; }
    }

    public class CheckboxStatistics
    {
        public double SelectionPercentage { get; set; }
    }

}
