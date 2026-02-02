using Application.Cache;
using Application.Write.Commands;
using Application.Write.Contracts;
using Domain.Entites;
using Domain.ValueObjects;
using SharedKernal;
using static Application.ErrorsMenu.ApplicationErrorsMenu.UserHandlersErrors;
using static Application.ErrorsMenu.ApplicationErrorsMenu.EmployeeHandlersErrors;
using Domain.Services;


namespace Application.Write.CommandHandlers
{
    public class UserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly UserService _userService;
        private readonly PermissionsCache _permissionsCache;


        public UserHandler(IUserRepository userRepository, 
                           PermissionsCache permissionsCache, 
                           UserService userService)
        {
            _userRepository = userRepository;
            _permissionsCache = permissionsCache;
            _userService = userService;
        }





        private async Task<Result<List<Permission>>> GetAndValidatePermissions(List<int> selectedIds)
        {
            var allPermissions = await _permissionsCache.GetAllAsync();
            
            var permissionsMap = allPermissions.ToDictionary(p => p.PermissionId);

            var permissionsToReturn = new List<Permission>();

            foreach (var id in selectedIds)
            {
                if (permissionsMap.TryGetValue(id, out var permission))
                    permissionsToReturn.Add(permission);
               
                else
                    return Result<List<Permission>>.Failure(PermissionNotFound(id));
                
            }

            return Result<List<Permission>>.Successful(permissionsToReturn);
        }
         

        public async Task<Result<bool>> AddHandle(UserAddCommand command)
        {
            var PasswordCreation = Password.Create(command.password);
            if(!PasswordCreation.IsSuccess) return Result<bool>.Failure(PasswordCreation.Error!);

            var PermissionsResult = await GetAndValidatePermissions(command.Permissions);
            if(!PermissionsResult.IsSuccess) return Result<bool>.Failure(PermissionsResult.Error!);
           

            var EmployeeValidationResult = await _userRepository.GetEmployeeValidationStatus(command.EmployeeId);
            if(!EmployeeValidationResult.Exists) return Result<bool>.Failure(EmployeeNotFound(command.EmployeeId));
            if(EmployeeValidationResult.UserNameExists) return Result<bool>.Failure(UserNameAlreadyExists(command.userName));
            if(!EmployeeValidationResult.IsHr) return Result<bool>.Failure(EmployeeNotHR(command.EmployeeId));
            if(EmployeeValidationResult.IsUser) return Result<bool>.Failure(EmployeeIsUser(command.EmployeeId));

            var UserCreation = _userService.AddUseCase(command.EmployeeId, command.userName, PasswordCreation.Value!, PermissionsResult.Value!);
            if(!UserCreation.IsSuccess) return Result<bool>.Failure(UserCreation.Error!);

            await _userRepository.AddAsync(UserCreation.Value!);
            return Result<bool>.Successful(true);
        }





        public async Task<Result<bool>> UpdateHandle(int Id, UserUpdateCommand command)
        {
            
            var user = await _userRepository.GetByIdAsync(Id);
            if(user == null) return Result<bool>.Failure(UserNotFound(Id));

            
            if(command.password != null)
            {
                var PasswordCreation = Password.Create(command.password);
                if(!PasswordCreation.IsSuccess) return Result<bool>.Failure(PasswordCreation.Error!);
                user.ChangePasswordHashing(PasswordCreation.Value!);
            }

            if(command.userName != null)
            {
                if(await _userRepository.IsUserNameExistsAsync(command.userName))
                    return Result<bool>.Failure(UserNameAlreadyExists(command.userName));
                
                user.ChangeUserName(command.userName);
            } 

            if(command.AddPermissions != null && command.AddPermissions.Count != 0)
            {
                var Result = await GetAndValidatePermissions(command.AddPermissions);
                var AddPermissionsList = Result.Value!;
                var AddResult = _userService.AddNewPermissionsCase(user, AddPermissionsList);
                if(!AddResult.IsSuccess) return Result<bool>.Failure(AddResult.Error!);
            }

            if(command.RemovePermissions != null && command.RemovePermissions.Count != 0)
            {
                var Result = await GetAndValidatePermissions(command.RemovePermissions);
                var RemovePermissionsList = Result.Value!;
                var RemoveResult = _userService.RemovePermissionsCase(user, RemovePermissionsList);
                if(!RemoveResult.IsSuccess) return Result<bool>.Failure(RemoveResult.Error!);
            }
            
            await _userRepository.UpdateAsync(user);
            return Result<bool>.Successful(true);
        }


        public async Task<Result<bool>> ActivationHandle(int Id, bool Activate)
        {
            var user = await _userRepository.GetByIdAsync(Id);
            if(user == null) return Result<bool>.Failure(UserNotFound(Id));

            Result<User> result = default;

            if (Activate) result = user.Activate();
            else result = user.Freeze();
            
            if(!result.IsSuccess) return Result<bool>.Failure(result.Error!);

            await _userRepository.UpdateAsync(user);
            return Result<bool>.Successful(true);
        }


        public async Task<Result<UserLoged>> LoginHandle(LoginCommand command)
        {
            var user = await _userRepository.GetUserByUserName(command.userName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(command.password, user.UserPasswordHashing.Value)) 
                return Result<UserLoged>.Failure(UserLoginInvalid());

            if (!user.IsActive) 
                return Result<UserLoged>.Failure(UserNotActive());
            
            var userLog = UserLog.Create(user);
            await _userRepository.AddNewLogAsync(userLog);

            var userDto = new UserLoged(user.UserId, user.UserName, user.PermissionsMask);
            return Result<UserLoged>.Successful(userDto);
        }
    }
}