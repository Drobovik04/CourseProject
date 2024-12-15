namespace CourseProject.Models
{
    public class TemplateTag
    {
        public int TemplateId { get; set; }
        public Template Template { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
