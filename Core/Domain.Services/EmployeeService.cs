using Doamin.Domain.ValueObjects;
using Domain.Entites;
using Domain.ValueObjects;
using SharedKernal;

namespace Domain.Services
{
    public class EmployeeService
    {
        public Result<Employee> AddNewEmployeeUseCase(
            string firstName, string secondName, string thirdName, string lastName,
            Nationality nationality, NationalNumber nationalNumber, BirthDate birthDate,
            Department department, JobTitleLevel jobTitleLevel, Employee? manager,
            PhoneNumber primaryPhone, EmailAddress primaryEmail, string currentAddress)
        {
          
            var workInfo = EmployeeWorkInfo.Create(department.DepartmentId, jobTitleLevel.JobTitleLevelId, manager?.EmployeeId);
            var phone = EmployeePhone.CreatePrimary(primaryPhone);
            var email = EmployeeEmail.CreatePrimary(primaryEmail);

           
            var code = EmployeeCodeGenration(nationalNumber.Value, birthDate);

           
            var managerValidation = CheckIfThisEmployeeIsValidManager(null!, workInfo, Enumerable.Empty<Employee>());
            if (!managerValidation.IsSuccess)
                return Result<Employee>.Failure(managerValidation.Error!);

          
            var employee = Employee.Create(
                firstName, secondName, thirdName, lastName,
                nationality.NationalityId, nationalNumber, code, birthDate,
                workInfo, phone, email, currentAddress);


            return Result<Employee>.Successful(employee);
        }

        public Result<Employee> UpdateEmployeeWorkInfoUseCase(
            Employee employee,
            Department? department,
            JobTitleLevel? jobTitleLevel,
            Employee? manager,
            IEnumerable<Employee> managerAncestors)
        {
            
            if (department == null && jobTitleLevel == null && manager == null)
                throw new NullReferenceException("department, jobTitleLevel, manager All Null");

            var targetDept = department ?? employee.CurrentWorkInfo.Department;
            var targetLevel = jobTitleLevel ?? employee.CurrentWorkInfo.JobTitleLevel;
            var targetManager = manager ?? employee.CurrentWorkInfo.Manager;

            var newWorkInfo = EmployeeWorkInfo.Create(targetDept.DepartmentId, targetLevel.JobTitleLevelId, targetManager?.EmployeeId);

            var managerCheck = CheckIfThisEmployeeIsValidManager(employee, newWorkInfo, managerAncestors);
            if (!managerCheck.IsSuccess) return Result<Employee>.Failure(managerCheck.Error!);

            return employee.ChangeWorkInformation(newWorkInfo);
        }



        public Result<Employee> AddPhoneUseCase(Employee employee, PhoneNumber phone, bool RequestedPrimary)
        {
            var newPhone = RequestedPrimary ? EmployeePhone.CreatePrimary(phone) : EmployeePhone.Create(phone);

            if (employee.Phones.Any(p => p.Value.Equals(phone)))
                return Result<Employee>.Failure(ErrorsMenu.ErrorsMenu.Employee.PhoneDuplicate);

            return employee.AddPhone(newPhone);
        }

        public Result<Employee> AddEmailUseCase(Employee employee, EmailAddress email, bool RequestedPrimary)
        {
            var newEmail = RequestedPrimary ? EmployeeEmail.CreatePrimary(email) : EmployeeEmail.Create(email);

            if (employee.Emails.Any(e => e.Value.Equals(email)))
                return Result<Employee>.Failure(ErrorsMenu.ErrorsMenu.Employee.EmailDuplicate);

            return employee.AddEmail(newEmail);
        }







        private Result<bool> CheckIfThisEmployeeIsValidManager(
        Employee employee,
        EmployeeWorkInfo workInfo,
        IEnumerable<Employee> managerAncestors)

        {

            if (workInfo.Manager == null) return Result<bool>.Successful(true);

            var managerWorkInfo = workInfo.Manager.CurrentWorkInfo;

            if (employee != null && employee.EmployeeId != 0 &&

            (workInfo.Manager.EmployeeId == employee.EmployeeId ||

            managerAncestors.Any(a => a.EmployeeId == employee.EmployeeId)))
            {
            return Result<bool>.Failure(ErrorsMenu.ErrorsMenu.Employee.CircularReference);
            }


            if (workInfo.Manager.Status != enEmployeeStatus.Active)
            return Result<bool>.Failure(ErrorsMenu.ErrorsMenu.Employee.ManagerNotActive);

            if (managerWorkInfo.Department.DepartmentId != workInfo.Department.DepartmentId)
            return Result<bool>.Failure(ErrorsMenu.ErrorsMenu.Employee.DifferentDepartment);

            if (managerWorkInfo.JobTitleLevel.JobGrade.CompareTo(workInfo.JobTitleLevel.JobGrade) <= 0)
            return Result<bool>.Failure(ErrorsMenu.ErrorsMenu.Employee.ManagerLowerGrade);

            return Result<bool>.Successful(true);

        }



        private static string EmployeeCodeGenration(string nationalNumber, BirthDate birthDate)
        {
            string rawData = $"{nationalNumber}-{birthDate.Value.Ticks}-{Guid.NewGuid()}";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
            return Convert.ToHexString(bytes).Substring(0, 8).ToUpper();
        }
    }
}