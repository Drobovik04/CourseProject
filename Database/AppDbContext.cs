using CourseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Database
{
    public class AppDbContext: IdentityDbContext<IdentityUser>
    {
        public DbSet<Form> Forms { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<FormAnswer> FormAnswers { get; set; }
        public DbSet<Answer> Answers { get; set; } // Для всех типов ответов (Text, Integer, Checkbox)
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Tag> Tags { get; set; }

        // Промежуточные таблицы (если нужны прямые запросы)
        public DbSet<FormAccess> FormAccesses { get; set; } // Необязательно, если только через связи
        public DbSet<FormTag> FormTags { get; set; } // Необязательно, если только через связи
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Связь Form ↔ AllowedUsers через FormAccess
            builder.Entity<FormAccess>()
                .HasKey(fa => new { fa.FormId, fa.UserId }); // Композитный ключ
            builder.Entity<FormAccess>()
                .HasOne(fa => fa.Form)
                .WithMany(f => f.AllowedUsers)
                .HasForeignKey(fa => fa.FormId);
            builder.Entity<FormAccess>()
                .HasOne(fa => fa.User)
                .WithMany()
                .HasForeignKey(fa => fa.UserId);

            // Связь Form ↔ Tags через FormTag
            builder.Entity<FormTag>()
                .HasKey(ft => new { ft.FormId, ft.TagId }); // Композитный ключ
            builder.Entity<FormTag>()
                .HasOne(ft => ft.Form)
                .WithMany(f => f.FormTags)
                .HasForeignKey(ft => ft.FormId);
            builder.Entity<FormTag>()
                .HasOne(ft => ft.Tag)
                .WithMany(t => t.FormTags)
                .HasForeignKey(ft => ft.TagId);

            // Связь Form ↔ Question
            builder.Entity<Question>()
                .HasOne(q => q.Form)
                .WithMany(f => f.Questions)
                .HasForeignKey(q => q.FormId);

            // Связь Form ↔ FormAnswer
            builder.Entity<FormAnswer>()
                .HasOne(fa => fa.Form)
                .WithMany(f => f.FormAnswers)
                .HasForeignKey(fa => fa.FormId);

            // Связь FormAnswer ↔ Answer
            builder.Entity<Answer>()
                .HasOne(a => a.FormAnswer)
                .WithMany(fa => fa.Answers)
                .HasForeignKey(a => a.FormAnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Form ↔ Comment
            builder.Entity<Comment>()
                .HasOne(c => c.Form)
                .WithMany(f => f.Comments)
                .HasForeignKey(c => c.FormId);

            // Связь Form ↔ Like
            builder.Entity<Like>()
                .HasOne(l => l.Form)
                .WithMany(f => f.Likes)
                .HasForeignKey(l => l.FormId);

            // Полиморфизм для Answer через дискриминатор
            builder.Entity<Answer>()
                .HasDiscriminator<string>("AnswerType")
                .HasValue<TextAnswer>("Text")
                .HasValue<IntegerAnswer>("Integer")
                .HasValue<CheckboxAnswer>("Checkbox");
        }
    }
}
