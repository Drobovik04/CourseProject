using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class QuestionStatisticsViewModel
    {
        public Question Question { get; set; }
        public double? AverageValue { get; set; }
        public string MostFrequentAnswer { get; set; }
    }
}
