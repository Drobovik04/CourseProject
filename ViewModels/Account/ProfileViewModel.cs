using CourseProject.Models;

namespace CourseProject.ViewModels.Account
{
    public class ProfileViewModel
    {
        public List<Models.Template> CreatedTemplates { get; set; } = new();
        public List<Models.Form> AnsweredTemplates { get; set; } = new();
        public string CurrentViewModeCreated { get; set; } = "Title";
        public string CurrentViewModeAnswered { get; set; } = "Title";
        public string SortColumnCreated { get; set; }
        public string SortOrderCreated { get; set; }
        public string SortColumnAnswered { get; set; }
        public string SortOrderAnswered {  get; set; }
        public string Tab {  get; set; }
    }
}
