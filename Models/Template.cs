using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models
{
    public class Template
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } // Markdown supported
        public string Topic { get; set; } // From predefined list
        public bool IsPublic { get; set; } // Public or restricted access
        public IdentityUser Author { get; set; } // Reference to the user
        public DateTime CreatedAt { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Tag> Tags { get; set; } // List of associated tags
    }
}
