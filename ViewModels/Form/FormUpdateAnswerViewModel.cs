using CourseProject.SupportClasses.Form;

namespace CourseProject.ViewModels.Form
{
    public class FormUpdateAnswerViewModel
    {
        public int FormId { get; set; }
        public List<AnswerUpdate> Answers { get; set; }
        public List<AnswerCreate> NewAnswers { get; set; }
    }
}
