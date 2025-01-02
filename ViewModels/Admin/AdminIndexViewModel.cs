namespace CourseProject.ViewModels.Admin
{
    public class AdminIndexViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLockedOut { get; set; }
    }
}
