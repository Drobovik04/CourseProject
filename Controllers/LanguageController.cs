using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Controllers
{
    public class LanguageController : Controller
    {
        private readonly ILogger<LanguageController> _logger;

        public LanguageController(ILogger<LanguageController> logger)
        {
            _logger = logger;
        }

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
