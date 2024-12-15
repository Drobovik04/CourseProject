using CourseProject.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CourseProject.ViewModels.Account
{
    public class LoginViewModel
    {
        [LocalizedRequired(ResourceKey = "Required")]
        public string UserNameOrEmail { get; set; }

        [LocalizedRequired(ResourceKey = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
