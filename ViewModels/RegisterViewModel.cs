using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CourseProject.ViewModels
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [MinLength(1)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizer = (IStringLocalizer<SharedResources>)validationContext.GetService(typeof(IStringLocalizer<SharedResources>));

            foreach (var property in validationContext.ObjectType.GetProperties())
            {
                var value = property.GetValue(this);

                // Проверка атрибута Required
                var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttribute != null && string.IsNullOrEmpty(value?.ToString()))
                {
                    var displayName = localizer[property.Name];
                    yield return new ValidationResult(localizer["Required", displayName], new[] { property.Name });
                }

                // Проверка атрибута MinLength
                var minLengthAttribute = property.GetCustomAttribute<MinLengthAttribute>();
                if (minLengthAttribute != null && (value?.ToString()?.Length ?? 0) < minLengthAttribute.Length)
                {
                    var displayName = localizer[property.Name];
                    yield return new ValidationResult(localizer["MinLength", displayName, minLengthAttribute.Length], new[] { property.Name });
                }

                // Проверка атрибута Compare
                var compareAttribute = property.GetCustomAttribute<CompareAttribute>();
                if (compareAttribute != null)
                {
                    var otherProperty = validationContext.ObjectType.GetProperty(compareAttribute.OtherProperty);
                    var otherValue = otherProperty?.GetValue(this);
                    if (!Equals(value, otherValue))
                    {
                        var displayName = localizer[property.Name];
                        var otherDisplayName = localizer[compareAttribute.OtherProperty];
                        yield return new ValidationResult(localizer["Compare", displayName, otherDisplayName], new[] { property.Name });
                    }
                }
            }
        }

    }
}
