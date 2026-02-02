using Application.Write.Commands;
using Application.Write.Contracts;
using Domain.Entites;
using SharedKernal;
using Application.Events;
using static Application.ErrorsMenu.ApplicationErrorsMenu.UserHandlersErrors;
using static Application.ErrorsMenu.ApplicationErrorsMenu.EmployeeHandlersErrors;
using Domain.ValueObjects;

namespace Application.Write.CommandHandlers
{
    public class UserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokensGentator;

        private readonly CrateNewUserEventHandler _crateNewUserEventHandler;

        public UserHandler(IUserRepository userRepository, ITokenService tokensGentator, CrateNewUserEventHandler crateNewUserEventHandler)
        {
            _userRepository = userRepository;
            _tokensGentator = tokensGentator;
            _crateNewUserEventHandler = crateNewUserEventHandler;
        }



        private async Task<Result<bool>> ChangeUserAction(Func<User, Result<User>> action, int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return Result<bool>.Failure(UserNotFound(id));

            var result = action(user); 
            if (!result.IsSuccess) return Result<bool>.Failure(result.Error);

            await _userRepository.UpdateAsync(user);
            return Result<bool>.Successful(true);
        }


        

        public async Task<Result<bool>> AddHandle(UserAddCommand command)
        {
            var validation = await _userRepository.GetEmployeeValidationStatus(command.EmployeeId, command.userName);
           

            if (validation == default || validation.Id == default) return Result<bool>.Failure(EmployeeNotFound(command.EmployeeId));
            if (validation.UserNameExists) return Result<bool>.Failure(UserNameAlreadyExists(command.userName));
            if (validation.IsUser) return Result<bool>.Failure(EmployeeIsUser(command.EmployeeId));


            var requestedRole = (enRole)command.role;
            var jobTitle = validation.JobCode;

            if (requestedRole == enRole.SystemAdmin)
            {
                if (jobTitle != "SYSAD") return Result<bool>.Failure(EmployeeNotSystemAdmin(command.EmployeeId));
            }
            else
            {
                if (jobTitle != "HRGEN") return Result<bool>.Failure(EmployeeNotHR(command.EmployeeId));
            }


            var user = User.Create(command.userName, (enRole)command.role, command.EmployeeId);
            
            var secureToken = _tokensGentator.CreateSecureToken(); 
            var activateToken = Token.CreateAccountActivation(secureToken.HashedToken, user);

            user.AddToken(activateToken);

            await _userRepository.AddAsync(user);



            var userEmail = validation.Email;
            var username = user.UserName;
            var rawToken = secureToken.PlainToken;

            await _crateNewUserEventHandler.Invoke(username, userEmail!, rawToken);
            return Result<bool>.Successful(true);
        }


        public async Task<Result<bool>> ChangeUsernameHandle(string username, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if(user == null) return Result<bool>.Failure(UserNotFound(username));

           if(await _userRepository.IsUserNameExistsAsync(username))
                    return Result<bool>.Failure(UserNameAlreadyExists(username));
                
                user.ChangeUserName(username);
            
            await _userRepository.UpdateAsync(user);
            return Result<bool>.Successful(true);
        }

        public async Task<Result<bool>> ChangePasswordHandle(ChangePasswordCommand command, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.status != enStatus.Active) return Result<bool>.Failure(UserNotActive(userId));

            if(!BCrypt.Net.BCrypt.Verify(command.OldPassword, user.UserPasswordHashing!.Value))
                return Result<bool>.Failure(PasswordNotMatch());

            var PasswordCreation = Password.Create(command.NewPassword);
            if(!PasswordCreation.IsSuccess) return Result<bool>.Failure(PasswordCreation.Error);

            user.SetPasswordHashing(PasswordCreation.Value!);

            await _userRepository.UpdateAsync(user);
            return Result<bool>.Successful(true);
        }

        
        public async Task<Result<bool>> ChangeToEmployeeManagmentHandle(int Id) => await ChangeUserAction(u => u.beEmployeeManager(), Id);
        public async Task<Result<bool>> ChangeToFinancialManagementHandle(int Id) => await ChangeUserAction(u => u.beFinancialManagement(), Id);


        public async Task<Result<bool>> ActivateHandle(int Id) => await ChangeUserAction(u => u.Activate(), Id);
        public async Task<Result<bool>> FreezeHandle(int Id) => await ChangeUserAction(u => u.Freeze(), Id);
    }
}