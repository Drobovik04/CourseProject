using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace CourseProject.Attributes
{
    public class LocalizedMaxLengthAttribute:MaxLengthAttribute
    {
        public string ResourceKey { get; set; }

        public LocalizedMaxLengthAttribute(int length, string resourceKey = "MaxLength") : base(length)
        {
            ResourceKey = resourceKey;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var localizer = (IStringLocalizer)validationContext.GetService(typeof(IStringLocalizer<SharedResources>));

            var fieldName = localizer?[validationContext.MemberName] ?? validationContext.MemberName;

            var errorMessage = localizer?[ResourceKey];

            var formattedMessage = string.Format(errorMessage, fieldName, Length);

            if (value is string str && str.Length > Length)
            {
                return new ValidationResult(formattedMessage, new List<string> { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
