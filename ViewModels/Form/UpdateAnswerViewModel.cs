namespace CourseProject.ViewModels.Form
{
    public class UpdateAnswerViewModel
    {
        public int FormId { get; set; }
        public List<AnswerUpdate> Answers { get; set; }
        public List<AnswerCreate> NewAnswers { get; set; }
    }
}
