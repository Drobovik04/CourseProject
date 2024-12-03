using Microsoft.SqlServer.Server;

namespace CourseProject.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<FormTag> FormTags { get; set; }
    }
}
