using CourseProject.Models;

namespace CourseProject.ViewModels.Account
{
    public class ProfileViewModel
    {
        public List<Models.Template> CreatedTemplates { get; set; } = new();
        public List<Models.Form> AnsweredTemplates { get; set; } = new();
    }
}
