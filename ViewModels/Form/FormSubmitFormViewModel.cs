using CourseProject.SupportClasses.Form;

namespace CourseProject.ViewModels.Form
{
    public class FormSubmitFormViewModel
    {
        public int TemplateId { get; set; }
        public Dictionary<int, QuestionAnswer> Questions { get; set; } = new();
        public Dictionary<int, AnswerInput> Answers { get; set; } = new();
        public bool SendCopyOnEmail { get; set; }
    }
}
