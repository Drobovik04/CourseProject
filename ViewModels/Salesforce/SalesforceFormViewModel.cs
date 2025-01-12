using CourseProject.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CourseProject.ViewModels.Salesforce
{
    public class SalesforceFormViewModel
    {
        public string UserId { get; set; }
        [LocalizedRequired]
        public string AccountName { get; set; }
        [LocalizedRequired]
        public string FirstName { get; set; }
        [LocalizedRequired]
        public string LastName { get; set; }
        [LocalizedRequired]
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
