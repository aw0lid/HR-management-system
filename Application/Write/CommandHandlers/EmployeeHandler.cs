using Application.Write.Commands;
using Application.Write.Contracts;
using Domain.Services;
using Domain.Entites;
using Domain.ValueObjects;
using SharedKernal;
using static Application.ErrorsMenu.ApplicationErrorsMenu.EmployeeHandlersErrors;
using Doamin.Domain.ValueObjects;
using Application.Cache;

namespace Application.Write.CommandHandlers
{
    public class EmployeeHandler
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly NationalitiesCache _nationalitiesCache;
        private readonly JobTitleLevelsCache _jobTitleLevelsCache;
        private readonly DepartmentsCache _departmentsCache;
        private readonly EmployeeService _employeeService;

        




        public EmployeeHandler(
            IEmployeeRepository employeeRepository, 
            NationalitiesCache nationalitiesCache,
            JobTitleLevelsCache jobTitleLevelsCache, 
            DepartmentsCache departmentsCache, 
            EmployeeService employeeService)
        {
            _employeeRepository = employeeRepository;
            _nationalitiesCache = nationalitiesCache;
            _jobTitleLevelsCache = jobTitleLevelsCache;
            _departmentsCache = departmentsCache;
            _employeeService = employeeService;
        }






        private async Task<Result<bool>> ExecuteEmployeeAction(int employeeId, Func<Employee, Result<Employee>> action)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) return Result<bool>.Failure(EmployeeNotFound(employeeId));

            var result = action(employee);
            if (!result.IsSuccess) return Result<bool>.Failure(result.Error!);

            await _employeeRepository.UpdateAsync(result.Value!);
            return Result<bool>.Successful(true);
        }



      
        public async Task<Result<bool>> AddHandle(EmployeeAddCommand command)
        {
            var phoneResult = PhoneNumber.Create(command.PhoneNumber);
            if (!phoneResult.IsSuccess) return Result<bool>.Failure(phoneResult.Error!);

            var emailResult = EmailAddress.Create(command.Email);
            if (!emailResult.IsSuccess) return Result<bool>.Failure(emailResult.Error!);

            var nationalNoResult = NationalNumber.Create(command.EmployeeNationalNumber);
            if (!nationalNoResult.IsSuccess) return Result<bool>.Failure(nationalNoResult.Error!);

            var birthDateResult = BirthDate.Create(command.EmployeeBirthDate);
            if (!birthDateResult.IsSuccess) return Result<bool>.Failure(birthDateResult.Error!);




            var (NationalNumberExists, phoneExists, emailExists) = await _employeeRepository.CheckConstraintsAsync(
                command.EmployeeNationalNumber, command.PhoneNumber, command.Email);

            if (NationalNumberExists)
                return Result<bool>.Failure(NationalNumberAlreadyExists(command.EmployeeNationalNumber));

            if (phoneExists)
                return Result<bool>.Failure(PhoneAlreadyExists(command.PhoneNumber));

            if (emailExists)
                return Result<bool>.Failure(EmailAlreadyExists(command.Email));



            var nationalities = await _nationalitiesCache.GetAllAsync();
            var nationality = nationalities.FirstOrDefault(n => n.NationalityId == command.EmployeeNationalityId);
            if (nationality == null) return Result<bool>.Failure(NationalityNotFound(command.EmployeeNationalityId));



            var departments = await _departmentsCache.GetAllAsync();
            var department = departments.FirstOrDefault(d => d.DepartmentId == command.DepartmentId);
            if (department == null) return Result<bool>.Failure(DepartmentNotFound(command.DepartmentId));

            var jobTitleLevels = await _jobTitleLevelsCache.GetAllAsync();
            var jobLevel = jobTitleLevels.FirstOrDefault(j => j.JobTitleLevelId == command.JobTitleLevelId);
            if (jobLevel == null) return Result<bool>.Failure(JobTitleLevelNotFound(command.JobTitleLevelId));

            Employee? manager = null;
            if (command.ManagerId.HasValue)
            {
                manager = await _employeeRepository.GetByIdAsync(command.ManagerId.Value);
                if (manager == null) return Result<bool>.Failure(ManagerNotFound(command.ManagerId!.Value));
            }
            




            Result<Employee> EmployeeCreation = _employeeService.AddNewEmployeeUseCase(
                  command.EmployeeFirstName, command.EmployeeSecondName, command.EmployeeThirdName, command.EmployeeLastName,
                  nationality, nationalNoResult.Value!, birthDateResult.Value!, department, jobLevel, manager,
                  phoneResult.Value!, emailResult.Value!, command.Address);


            if(!EmployeeCreation.IsSuccess) return Result<bool>.Failure(EmployeeCreation.Error!);


            await _employeeRepository.AddAsync(EmployeeCreation.Value!);
            return Result<bool>.Successful(true);
        }

     


        public Task<Result<bool>> ActiveEmployeeHandle(int id) => ExecuteEmployeeAction(id, e => e.Activate());
        public Task<Result<bool>> SuspendEmployeeHandle(int id) => ExecuteEmployeeAction(id, e => e.Suspended());
        public Task<Result<bool>> TerminateEmployeeHandle(int id) => ExecuteEmployeeAction(id, e => e.Terminate());




        public async Task<Result<bool>> UpdatePersonalInfoHandle(int Id, EmployeePersonalInfoUpdateCommand command)
        {
            var employee = await _employeeRepository.GetByIdAsync(Id);
            if (employee == null) return Result<bool>.Failure(EmployeeNotFound(Id));

            
            Nationality? nationality = null;
            if (command.EmployeeNationalityId.HasValue)
            {
                var nationalities = await _nationalitiesCache.GetAllAsync();
                nationality = nationalities.FirstOrDefault(n => n.NationalityId == command.EmployeeNationalityId);
                if (nationality == null) return Result<bool>.Failure(NationalityNotFound(command.EmployeeNationalityId.Value));
            }

           
            NationalNumber? nationalNo = null;
            if (!string.IsNullOrWhiteSpace(command.EmployeeNationalNumber) &&
                employee.EmployeeNationalNumber?.Value != command.EmployeeNationalNumber)
            {
                var nationalNoResult = NationalNumber.Create(command.EmployeeNationalNumber);
                if (!nationalNoResult.IsSuccess) return Result<bool>.Failure(nationalNoResult.Error!);

                var isNationalNumberTaken = await _employeeRepository.IsNationalNumberExistsAsync(command.EmployeeNationalNumber);
                if (isNationalNumberTaken) return Result<bool>.Failure(NationalNumberAlreadyExists(command.EmployeeNationalNumber));

                nationalNo = nationalNoResult.Value;
            }

         
            BirthDate? birthDate = null;
            if (command.EmployeeBirthDate.HasValue)
            {
                var birthDateResult = BirthDate.Create(command.EmployeeBirthDate.Value.Date);
                if (!birthDateResult.IsSuccess) return Result<bool>.Failure(birthDateResult.Error!);
                birthDate = birthDateResult.Value;
            }


            if (!string.IsNullOrWhiteSpace(command.EmployeeFirstName)) employee.ChangeFirstName(command.EmployeeFirstName);
            if (!string.IsNullOrWhiteSpace(command.EmployeeSecondName)) employee.ChangeSecondName(command.EmployeeSecondName);
            if (!string.IsNullOrWhiteSpace(command.EmployeeThirdName)) employee.ChangeThirdName(command.EmployeeThirdName);
            if (!string.IsNullOrWhiteSpace(command.EmployeeLastName)) employee.ChangeLastName(command.EmployeeLastName);

            if (nationality != null) employee.ChangeNationality(nationality);
            if (nationalNo != null) employee.ChangeNationalNumber(nationalNo);
            if (birthDate != null) employee.ChangeBirthDate(birthDate);
            if (!string.IsNullOrWhiteSpace(command.Address)) employee.ChangeAddress(command.Address);

           
            if (command.EmployeeGender.HasValue)
            {
                if (command.EmployeeGender.Value == 1) employee.ChangeMale();
                else employee.ChangeFemale();
            }

           
            await _employeeRepository.UpdateAsync(employee);

            return Result<bool>.Successful(true);
        }


        






        public async Task<Result<bool>> UpdateWorkInfoHandle(int Id, EmployeeWorkInfoUpdateCommand command)
        {
           
            var employee = await _employeeRepository.GetByIdAsync(Id);
            if (employee == null)
                return Result<bool>.Failure(EmployeeNotFound(Id));

            
            Department? dept = null;
            if (command.DepartmentId.HasValue)
            {
                var departments = await _departmentsCache.GetAllAsync();
                dept = departments.FirstOrDefault(d => d.DepartmentId == command.DepartmentId);
                if (dept == null) return Result<bool>.Failure(DepartmentNotFound(command.DepartmentId.Value));
            }

            JobTitleLevel? level = null;
            if (command.JobTitleLevelId.HasValue)
            {
                var jobTitleLevels = await _jobTitleLevelsCache.GetAllAsync();
                level = jobTitleLevels.FirstOrDefault(j => j.JobTitleLevelId == command.JobTitleLevelId);
                if (level == null) return Result<bool>.Failure(JobTitleLevelNotFound(command.JobTitleLevelId.Value));
            }

            Employee? manager = null;
            IEnumerable<Employee>? ManagersOfManager = null;
            if (command.ManagerId.HasValue)
            {
                manager = await _employeeRepository.GetByIdAsync(command.ManagerId.Value);
                if (manager == null) return Result<bool>.Failure(EmployeeNotFound(command.ManagerId.Value));

                ManagersOfManager = await _employeeRepository.GetAllManagersAsync(manager.EmployeeId);
            }


           var Result = _employeeService.UpdateEmployeeWorkInfoUseCase(employee, dept, level, manager, ManagersOfManager!);

            if (!Result.IsSuccess)
                return Result<bool>.Failure(Result.Error!);

            await _employeeRepository.UpdateAsync(Result.Value!);
            return Result<bool>.Successful(true);     
        }
    }
}