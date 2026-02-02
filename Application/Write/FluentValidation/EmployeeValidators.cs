using Application.Write.Commands;
using FluentValidation;

namespace Application.Write.FluentValidation
{
    public class EmployeeAddValidator : AbstractValidator<EmployeeAddCommand>
    {
        public EmployeeAddValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop; 
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.EmployeeFirstName).NotEmpty().WithErrorCode(ValidationKeys.Required);
            RuleFor(x => x.EmployeeSecondName).NotEmpty().WithErrorCode(ValidationKeys.Required);
            RuleFor(x => x.EmployeeThirdName).NotEmpty().WithErrorCode(ValidationKeys.Required);
            RuleFor(x => x.EmployeeLastName).NotEmpty().WithErrorCode(ValidationKeys.Required);

        
            RuleFor(x => x.EmployeeBirthDate)
                .NotEmpty().WithErrorCode(ValidationKeys.Required);
                
                
            RuleFor(x => x.EmployeeNationalNumber)
                .NotEmpty().WithErrorCode(ValidationKeys.Required);

            RuleFor(x => x.EmployeeBirthDate)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .LessThanOrEqualTo(DateTime.Today).WithErrorCode(ValidationKeys.InvalidFormat);

            
            RuleFor(x => x.EmployeeNationalityId).GreaterThan(0).WithErrorCode(ValidationKeys.InvalidSelection);
            RuleFor(x => x.DepartmentId).GreaterThan(0).WithErrorCode(ValidationKeys.InvalidSelection);
            RuleFor(x => x.JobTitleLevelId).GreaterThan(0).WithErrorCode(ValidationKeys.InvalidSelection);
            RuleFor(x => x.EmployeeGender).InclusiveBetween((short)1, (short)2).WithErrorCode(ValidationKeys.InvalidRange);
        }
    }











    public class EmployeePersonalInfoUpdateValidator : AbstractValidator<EmployeePersonalInfoUpdateCommand>
    {
        public EmployeePersonalInfoUpdateValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop; 
            
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.EmployeeFirstName) ||
                           !string.IsNullOrWhiteSpace(x.EmployeeSecondName) ||
                           !string.IsNullOrWhiteSpace(x.EmployeeThirdName) ||
                           !string.IsNullOrWhiteSpace(x.EmployeeLastName) ||
                           !string.IsNullOrWhiteSpace(x.EmployeeNationalNumber) ||
                           x.EmployeeBirthDate.HasValue ||
                           x.EmployeeGender.HasValue ||
                           x.EmployeeNationalityId.HasValue ||
                           !string.IsNullOrWhiteSpace(x.Address))
                .WithErrorCode(ValidationKeys.AtLeastOneField);

           
            RuleFor(x => x.EmployeeFirstName)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .When(x => x.EmployeeFirstName != null);

            RuleFor(x => x.EmployeeSecondName)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .When(x => x.EmployeeSecondName != null);

            RuleFor(x => x.EmployeeThirdName)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .When(x => x.EmployeeThirdName != null);

            RuleFor(x => x.EmployeeLastName)
                .NotEmpty().WithErrorCode(ValidationKeys.Required)
                .When(x => x.EmployeeLastName != null);

            RuleFor(x => x.EmployeeBirthDate)
                .LessThanOrEqualTo(DateTime.Today).WithErrorCode(ValidationKeys.InvalidFormat)
                .When(x => x.EmployeeBirthDate.HasValue);
        }
    }

    

    public class EmployeeWorkInfoUpdateValidator : AbstractValidator<EmployeeWorkInfoUpdateCommand>
    {
        public EmployeeWorkInfoUpdateValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop; 
            
            RuleFor(x => x)
                .Must(x => x.DepartmentId.HasValue || x.JobTitleLevelId.HasValue || x.ManagerId.HasValue)
                .WithErrorCode(ValidationKeys.AtLeastOneField);

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithErrorCode(ValidationKeys.InvalidSelection)
                .When(x => x.DepartmentId.HasValue);

            RuleFor(x => x.JobTitleLevelId)
                .GreaterThan(0).WithErrorCode(ValidationKeys.InvalidSelection)
                .When(x => x.JobTitleLevelId.HasValue);

            RuleFor(x => x.ManagerId)
                .GreaterThan(0).WithErrorCode(ValidationKeys.InvalidSelection)
                .When(x => x.ManagerId.HasValue);
        }
    }
}