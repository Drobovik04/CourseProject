using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace CourseProject.Attributes
{
    public class LocalizedCompareAttribute:CompareAttribute
    {
        public string ResourceKey { get; set; }

        public LocalizedCompareAttribute(string otherProperty, string resourceKey = "Compare") : base(otherProperty)
        {
            ResourceKey = resourceKey;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var localizer = (IStringLocalizer)validationContext.GetService(typeof(IStringLocalizer<SharedResources>));

            var currentFieldName = localizer?[validationContext.MemberName] ?? validationContext.MemberName;
            var otherProperty = validationContext.ObjectType.GetProperty(OtherProperty);
            var otherFieldName = localizer?[OtherProperty] ?? OtherProperty;

            var errorMessage = localizer?[ResourceKey];

            var formattedMessage = string.Format(errorMessage, currentFieldName, otherFieldName);

            var otherValue = otherProperty?.GetValue(validationContext.ObjectInstance);
            if (!Equals(value, otherValue))
            {
                return new ValidationResult(formattedMessage, new List<string> { validationContext.MemberName, OtherProperty });
            }

            return ValidationResult.Success;
        }
    }
}
