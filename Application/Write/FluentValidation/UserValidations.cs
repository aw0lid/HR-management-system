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
                .MinimumLength(3).WithErrorCode(ValidationKeys.InvalidFormat)
                .MaximumLength(20).WithErrorCode(ValidationKeys.InvalidFormat);

            RuleFor(x => x.role).InclusiveBetween((short)1, (short)3).WithErrorCode(ValidationKeys.InvalidRange);
        }
    }



    public class UserUpdateValidator : AbstractValidator<UserUpdateCommand>
    {
        public UserUpdateValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.userName))
                .WithErrorCode(ValidationKeys.AtLeastOneField);

            RuleFor(x => x.userName)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .MaximumLength(20).WithErrorCode(ValidationKeys.TooLong)
                .When(x => x.userName != null);  
        }
    }


    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.userName)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .MaximumLength(20).WithErrorCode(ValidationKeys.TooLong);

            RuleFor(x => x.password)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .MinimumLength(8).WithErrorCode(ValidationKeys.TooShort);
        }
    }



    public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
    {
        public virtual bool BeValidPassword(string password)
        {
            return password.Length >= 8;
        }

        public ChangePasswordValidator()
        {
            RuleFor(x => x.OldPassword).NotEmpty().WithErrorCode(ValidationKeys.Required);
            
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .MinimumLength(8).WithErrorCode(ValidationKeys.InvalidFormat)
                .NotEqual(x => x.OldPassword).WithErrorCode(ValidationKeys.InvalidFormat);
        }
    }


    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithErrorCode(ValidationKeys.Required);
            
            RuleFor(x => x.Token).NotEmpty().WithErrorCode(ValidationKeys.InvalidFormat);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .MinimumLength(8).WithErrorCode(ValidationKeys.InvalidFormat);
        }
    }


    public class ActivateUserValidator : AbstractValidator<ActivateUserCommand>
    {
        public ActivateUserValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithErrorCode(ValidationKeys.Required);
            
            RuleFor(x => x.Token).NotEmpty().WithErrorCode(ValidationKeys.InvalidFormat);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithErrorCode(ValidationKeys.InvalidFormat)
                .MinimumLength(8).WithErrorCode(ValidationKeys.Required);
        }
    }
}