using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class ProfileViewModel
    {
        public List<Form> CreatedForms { get; set; } = new();
        public List<FormAnswer> AnsweredForms { get; set; } = new();
    }
}
