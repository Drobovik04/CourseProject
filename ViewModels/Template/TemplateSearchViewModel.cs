namespace CourseProject.ViewModels.Template
{
    public class TemplateSearchViewModel
    {
        public List<Models.Template> Templates { get; set; } = new();
        public string CurrentViewMode { get; set; } = "Title";
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
    }
}
