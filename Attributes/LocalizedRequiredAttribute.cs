using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace CourseProject.Attributes
{
    public class LocalizedRequiredAttribute : RequiredAttribute
    {
        public string ResourceKey { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var localizer = (IStringLocalizer)validationContext.GetService(typeof(IStringLocalizer<SharedResources>));

            var fieldName = localizer?[validationContext.MemberName] ?? validationContext.MemberName;

            var errorMessage = localizer?[ResourceKey ?? "Required"];

            var formattedMessage = string.Format(errorMessage, fieldName);

            return string.IsNullOrEmpty(value?.ToString()) ? new ValidationResult(formattedMessage, new List<string> { validationContext.MemberName }) : ValidationResult.Success;
        }
    }
}
