using CourseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Database
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Template> Templates { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Topic> Topics { get; set; }

        public DbSet<TemplateAccess> TemplateAccesses { get; set; }
        public DbSet<TemplateTag> TemplateTags { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Template ↔ AllowedUsers via TemplateAccess
            builder.Entity<TemplateAccess>()
                .HasKey(x => new { x.TemplateId, x.UserId });
            builder.Entity<TemplateAccess>()
                .HasOne(x => x.Template)
                .WithMany(x => x.AllowedUsers)
                .HasForeignKey(x => x.TemplateId);
            builder.Entity<TemplateAccess>()
                .HasOne(fa => fa.User)
                .WithMany()
                .HasForeignKey(fa => fa.UserId);

            // Template ↔ Tags via TemplateTag
            builder.Entity<TemplateTag>()
                .HasKey(x => new { x.TemplateId, x.TagId });
            builder.Entity<TemplateTag>()
                .HasOne(x => x.Template)
                .WithMany(x => x.TemplateTags)
                .HasForeignKey(x => x.TemplateId);
            builder.Entity<TemplateTag>()
                .HasOne(x => x.Tag)
                .WithMany(x => x.TemplateTags)
                .HasForeignKey(x => x.TagId);

            // Form ↔ Question
            builder.Entity<Question>()
                .HasOne(x => x.Template)
                .WithMany(x => x.Questions)
                .HasForeignKey(q => q.TemplateId);

            // Template ↔ Form
            builder.Entity<Form>()
                .HasOne(x => x.Template)
                .WithMany(x => x.Forms)
                .HasForeignKey(x => x.TemplateId);

            // Form ↔ Answer
            builder.Entity<Answer>()
                .HasOne(x => x.Form)
                .WithMany(x => x.Answers)
                .HasForeignKey(a => a.FormId)
                .OnDelete(DeleteBehavior.Restrict);

            // Template ↔ Comment
            builder.Entity<Comment>()
                .HasOne(x => x.Template)
                .WithMany(x => x.Comments)
                .HasForeignKey(c => c.TemplateId);

            // Template ↔ Like
            builder.Entity<Like>()
                .HasOne(x => x.Template)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.TemplateId);

            builder.Entity<Like>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            // Template ↔ Topics
            builder.Entity<Topic>()
                .HasMany(x => x.Templates)
                .WithOne(x => x.Topic)
                .HasForeignKey(x => x.TopicId)
                .OnDelete(DeleteBehavior.SetNull);

            // Polymorphism for Answer
            builder.Entity<Answer>()
                .HasDiscriminator<string>("AnswerType")
                .HasValue<TextAnswer>("Text")
                .HasValue<IntegerAnswer>("Integer")
                .HasValue<CheckboxAnswer>("Checkbox");
        }
    }
}
