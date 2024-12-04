using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CourseProject
{
    public class LocalizedIdentityErrorDescriber:IdentityErrorDescriber
    {
        private readonly IStringLocalizer<SharedResources> _localizer;

        public LocalizedIdentityErrorDescriber(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = _localizer["DuplicateUserName", userName] ?? $"Username '{userName}' is already taken"
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = _localizer["DuplicateEmail", email] ?? $"Email '{email}' is already taken"
            };
        }

        public override IdentityError InvalidUserName(string? userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = _localizer["InvalidUserName", userName] ?? $"The username '{userName}' is invalid"
            };
        }

        public override IdentityError InvalidEmail(string? email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = _localizer["InvalidEmail", email] ?? $"The email '{email}' is invalid"
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = _localizer["PasswordTooShort", length] ?? $"Password must be at least {length} characters long"
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = _localizer["PasswordRequiresNonAlphanumeric"] ?? "Passwords must have at least one non-alphanumeric character"
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = _localizer["PasswordRequiresDigit"] ?? "Passwords must have at least one digit ('0'-'9')"
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = _localizer["PasswordRequiresLower"] ?? "Passwords must have at least one lowercase ('a'-'z')"
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = _localizer["PasswordRequiresUpper"] ?? "Passwords must have at least one uppercase ('A'-'Z')"
            };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyHasPassword),
                Description = _localizer["UserAlreadyHasPassword"] ?? "User already has a password set"
            };
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError
            {
                Code = nameof(UserLockoutNotEnabled),
                Description = _localizer["UserLockoutNotEnabled"] ?? "Lockout is not enabled for this user"
            };
        }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return new IdentityError
            {
                Code = nameof(RecoveryCodeRedemptionFailed),
                Description = _localizer["RecoveryCodeRedemptionFailed"] ?? "Recovery code redemption failed"
            };
        }
    }
}
