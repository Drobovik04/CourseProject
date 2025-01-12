using CourseProject.Database;
using CourseProject.Utilities;
using CourseProject.ViewModels.Salesforce;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CourseProject.Controllers
{
    public class SalesforceController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly SalesforceInteraction _salesforce;

        public SalesforceController(IConfiguration configuration, AppDbContext context, SalesforceInteraction salesforce)
        {
            _configuration = configuration;
            _context = context;
            _salesforce = salesforce;
        }
        [HttpGet]
        public async Task<IActionResult> Create(string userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            SalesforceFormViewModel model;

            if (!string.IsNullOrEmpty(user.SalesforceAccountId))
            {
                model = await _salesforce.GetSalesforceDataAsync(user.SalesforceAccountId);
            }
            else
            {
                model = new SalesforceFormViewModel
                {
                    UserId = userId,
                    AccountName = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber
                };
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SalesforceFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            var data = await _salesforce.Create(model);

            var user = _context.Users.Find(model.UserId);
            user.SalesforceAccountId = data.AccountId;
            user.SalesforceContactId = data.ContactId;

            _context.SaveChanges();

            return RedirectToAction("Index", "Account");
        }
    }
}
