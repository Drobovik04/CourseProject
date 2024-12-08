using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class FormDetailsViewModel
    {
        public Form Form { get; set; }
        public List<FormAnswer> UserAnswers { get; set; }
        public bool IsEditable {  get; set; }
    }
}
