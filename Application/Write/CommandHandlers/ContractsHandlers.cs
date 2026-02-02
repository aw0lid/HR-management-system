using Application.Write.Contracts;
using Domain.Entites;
using Domain.Services;
using Domain.ValueObjects;
using SharedKernal;
using static Application.ErrorsMenu.ApplicationErrorsMenu;

namespace Application.Write.CommandHandlers
{
    public class EmployeeContactsHandler
    {
        private IEmployeeRepository _employeeRepository = default!;
        private EmployeeService _employeeService = default!;




        public EmployeeContactsHandler(IEmployeeRepository employeeRepository, EmployeeService employeeService)
        {
            _employeeRepository = employeeRepository;
            _employeeService = employeeService;
        }




        private async Task<Result<bool>> ExecuteEmployeeActionAsync(int employeeId, Func<int, Task<Employee?>> employeeLoader, 
                                                                    Func<Employee, Result<Employee>> domainAction)
        {
            var employee = await employeeLoader(employeeId);

            if (employee == null)
                return Result<bool>.Failure(EmployeeHandlersErrors.EmployeeNotFound(employeeId));

           
            var result = domainAction(employee);

            if (!result.IsSuccess)
                return Result<bool>.Failure(result.Error!);

           
            await _employeeRepository.UpdateAsync(employee);
            return Result<bool>.Successful(true);
        }









        public async Task<Result<bool>> AddPhoneHandle(int EmployeeId, string phoneNumber, bool RequestedPrimary)
        {
            var CreationResult = PhoneNumber.Create(phoneNumber);
            if (!CreationResult.IsSuccess) return Result<bool>.Failure(CreationResult.Error!);

            
            return await ExecuteEmployeeActionAsync(
                EmployeeId,
                id => _employeeRepository.GetWithPhones(id)!,
                e => _employeeService.AddPhoneUseCase(e, CreationResult.Value!, RequestedPrimary)
            );
        }

        public async Task<Result<bool>> UpdatePhoneHandle(int Id, int EmployeeId, string NewPhone)
        {
            var PhoneNumberCreationResult = PhoneNumber.Create(NewPhone);

            if (!PhoneNumberCreationResult.IsSuccess)
                return Result<bool>.Failure(PhoneNumberCreationResult.Error!);

            if(await _employeeRepository.IsPhoneNumberExistsAsync(NewPhone))
                return Result<bool>.Failure(EmployeeHandlersErrors.PhoneAlreadyExists(NewPhone));


            return await ExecuteEmployeeActionAsync(
               EmployeeId,
               id => _employeeRepository.GetWithPhones(EmployeeId)!,
               e => e.UpdatePhone(PhoneNumberCreationResult.Value!, Id)
           );
        }


        public async Task<Result<bool>> FreezePhoneHandle(int Id, int EmployeeId)
        {
            return await ExecuteEmployeeActionAsync(EmployeeId, id => _employeeRepository.GetWithPhones(id)!,
                e => e.FreezePhone(Id));
        }


        public async Task<Result<bool>> SetPrimaryPhoneHandle(int Id, int EmployeeId)
        {
            return await ExecuteEmployeeActionAsync(EmployeeId, id => _employeeRepository.GetWithPhones(id)!,
               e => e.SetPrimaryPhone(Id));
        }










        public async Task<Result<bool>> AddEmailHandle(int EmployeeId, string emailAddress, bool RequestedPrimary)
        {
            var CreationResult = EmailAddress.Create(emailAddress);
            if (!CreationResult.IsSuccess) return Result<bool>.Failure(CreationResult.Error!);

            
            return await ExecuteEmployeeActionAsync(
                EmployeeId,
                id => _employeeRepository.GetWithEmails(id)!,
                e => _employeeService.AddEmailUseCase(e, CreationResult.Value!, RequestedPrimary)
            );
        }

        public async Task<Result<bool>> UpdateEmailHandle(int Id, int EmployeeId, string NewEmail)
        {
             var CreationResult = EmailAddress.Create(NewEmail);

            if (!CreationResult.IsSuccess)
                return Result<bool>.Failure(CreationResult.Error!);

            if(await _employeeRepository.IsEmailExistsAsync(NewEmail))
                return Result<bool>.Failure(EmployeeHandlersErrors.PhoneAlreadyExists(NewEmail));


            return await ExecuteEmployeeActionAsync(
               EmployeeId,
               id => _employeeRepository.GetWithEmails(id)!,
               e => e.UpdateEmail(CreationResult.Value!, Id)
           );
        }

        public async Task<Result<bool>> FreezeEmailHandle(int Id, int EmployeeId)
        {
             return await ExecuteEmployeeActionAsync(EmployeeId, id => _employeeRepository.GetWithEmails(id)!,
                e => e.FreezeEmail(Id));
        }

        public async Task<Result<bool>> SetPrimaryEmailHandle(int Id, int EmployeeId)
        {

            return await ExecuteEmployeeActionAsync(EmployeeId, id => _employeeRepository.GetWithEmails(id)!,
                e => e.SetPrimaryEmail(Id));
        }

    }
}