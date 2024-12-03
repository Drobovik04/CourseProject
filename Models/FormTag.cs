namespace CourseProject.Models
{
    public class FormTag
    {
        public int FormId { get; set; }
        public Form Form { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
