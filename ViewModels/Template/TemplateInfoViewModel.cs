namespace CourseProject.ViewModels.Template
{
    public class TemplateInfoViewModel
    {
        public TemplateDetailsViewModel Details { get; set; }
        public TemplateAnswersViewModel Answers { get; set; }
        public TemplateStatisticsViewModel Statistics { get; set; }
        public string CurrentTab { get; set; }
    }
}
