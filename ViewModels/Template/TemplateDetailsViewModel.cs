using CourseProject.Models;

namespace CourseProject.ViewModels.Template
{
    public class TemplateDetailsViewModel
    {
        public Models.Template Template { get; set; }
        public List<Models.Form> UserAnswers { get; set; }
        public bool IsEditable { get; set; }
        public List<Question> Questions { get; set; }
        public bool UserHasLiked { get; set; }
    }
}
