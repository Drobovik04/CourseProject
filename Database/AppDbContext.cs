using CourseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Database
{
    public class AppDbContext: IdentityDbContext<IdentityUser>
    {
        public DbSet<Template> Templates { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormAnswer> FormAnswers { get; set; }
        public DbSet<TextFormAnswer> TextFormAnswers { get; set; }
        public DbSet<IntegerFormAnswer> IntegerFormAnswers { get; set; }
        public DbSet<CheckboxFormAnswer> CheckboxFormAnswers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FormAnswer>()
                .HasDiscriminator<string>("AnswerType")
                .HasValue<TextFormAnswer>("Text")
                .HasValue<IntegerFormAnswer>("Integer")
                .HasValue<CheckboxFormAnswer>("Checkbox");
            builder.Entity<Template>()
                .HasMany(t => t.Tags)
                .WithMany(t => t.Templates)
                .UsingEntity(j => j.ToTable("TemplateTags"));
        }
    }
}
