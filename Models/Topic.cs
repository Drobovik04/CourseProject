namespace CourseProject.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Template> Templates { get; set; } = new List<Template>();
    }
}
