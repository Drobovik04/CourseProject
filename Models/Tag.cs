namespace CourseProject.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } // E.g., "Education," "Survey," etc.
        public ICollection<Template> Templates { get; set; } // Many-to-many relationship
    }
}
