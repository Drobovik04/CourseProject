using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Controllers
{
    public class TemplateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
