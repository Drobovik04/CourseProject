namespace CourseProject.ViewModels.Template
{
    public class TemplateAnswersViewModel
    {
        public int TemplateId { get; set; }
        public List<Models.Form> Forms { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
    }
}
