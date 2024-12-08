namespace CourseProject.ViewModels
{
    public class SubmitFormViewModel
    {
        public int FormId { get; set; }
        public Dictionary<int, QuestionAnswerViewModel> Questions { get; set; } = new();
        public Dictionary<int, AnswerInputViewModel> Answers { get; set; } = new();
    }
}
