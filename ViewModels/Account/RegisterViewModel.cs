using CourseProject.Attributes;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CourseProject.ViewModels.Account
{
    public class RegisterViewModel
    {
        [LocalizedRequired(ResourceKey = "Required")]
        public string Email { get; set; }

        [LocalizedRequired(ResourceKey = "Required")]
        [LocalizedMinLength(1, ResourceKey = "MinLength")]
        public string UserName { get; set; }

        [LocalizedRequired(ResourceKey = "Required")]
        [LocalizedMinLength(8, ResourceKey = "MinLength")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [LocalizedRequired(ResourceKey = "Required")]
        [LocalizedCompare("Password", ResourceKey = "Compare")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
