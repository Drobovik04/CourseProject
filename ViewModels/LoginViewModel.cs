using System.ComponentModel.DataAnnotations;

namespace CourseProject.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UserNameOrEmail { get; set; }  // Поле для логина или email

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
