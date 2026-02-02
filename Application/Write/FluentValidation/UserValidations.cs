using Application.Write.Commands;
using FluentValidation;

namespace Application.Write.FluentValidation
{
    public class UserAddValidator : AbstractValidator<UserAddCommand>
    {
        public UserAddValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithErrorCode(ValidationKeys.InvalidSelection);

            RuleFor(x => x.userName)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .MinimumLength(3).WithErrorCode(ValidationKeys.TooShort)
                .MaximumLength(20).WithErrorCode(ValidationKeys.TooLong);

            RuleFor(x => x.password)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .MinimumLength(6).WithErrorCode(ValidationKeys.TooShort);

            RuleFor(x => x.Permissions)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .Must(p => p != null && p.Count > 0).WithErrorCode(ValidationKeys.InvalidSelection);
        }
    }



    public class UserUpdateValidator : AbstractValidator<UserUpdateCommand>
    {
        public UserUpdateValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.userName) ||
                           !string.IsNullOrWhiteSpace(x.password) ||
                           (x.AddPermissions != null && x.AddPermissions.Any()) ||
                           (x.RemovePermissions != null && x.RemovePermissions.Any()))
                .WithErrorCode(ValidationKeys.AtLeastOneField);

            RuleFor(x => x.userName)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .MaximumLength(20).WithErrorCode(ValidationKeys.TooLong)
                .When(x => x.userName != null);

            RuleFor(x => x.password)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .MinimumLength(8).WithErrorCode(ValidationKeys.TooShort)
                .When(x => x.password != null);

            RuleForEach(x => x.AddPermissions)
                .GreaterThan(0).WithErrorCode(ValidationKeys.InvalidSelection)
                .When(x => x.AddPermissions != null);

            RuleForEach(x => x.RemovePermissions)
                .GreaterThan(0).WithErrorCode(ValidationKeys.InvalidSelection)
                .When(x => x.RemovePermissions != null);
        }
    }
}