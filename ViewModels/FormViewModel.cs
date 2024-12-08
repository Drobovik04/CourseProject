using CourseProject.Models;

namespace CourseProject.ViewModels
{
    public class FormViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; } // Markdown поддержка
        public string? ImageUrl { get; set; } // URL изображения
        public AccessType AccessType { get; set; } // Публичный/ограниченный доступ
        public List<string>? Tags { get; set; } = new(); // Теги
        public List<string>? AllowedUserNames { get; set; } = new(); // Email разрешённых пользователей
        public List<QuestionViewModel> Questions { get; set; } = new(); // Список вопросов
    }
}
