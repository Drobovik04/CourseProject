namespace CourseProject.ViewModels.Template
{
    public class TemplateMainViewModel
    {
        public List<Models.Template> LatestTemplates { get; set; }
        public List<Models.Template> FiveMostPopularTemplates { get; set; }
        public List<TagForCloud> TagCloud { get; set; }
        public string SortColumnTab1 { get; set; }
        public string SortColumnTab2 { get; set; }
        public string SortOrderTab1 { get; set; }
        public string SortOrderTab2 { get; set; }
        public string ViewModeTab1 { get; set; }
        public string ViewModeTab2 { get; set; }
        public string CurrentTab { get; set; }
    }
}
