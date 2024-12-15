namespace CourseProject.ViewModels.Form
{
    public class ViewAnswerViewModel
    {
        public Models.Template Template { get; set; }
        public Models.Form Form { get; set; }
        public string UserId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<QuestionAnswerPair> QuestionAnswerMap { get; set; }
    }
}
