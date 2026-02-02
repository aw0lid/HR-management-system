using SharedKernal;
using Domain.ValueObjects;
using static Domain.ErrorsMenu.ErrorsMenu.User;


namespace Domain.Entites
{
    public class User
    {
        public int UserId { get; private set; }
        public string UserName { get; private set; } = null!;
        public Password UserPasswordHashing { get; private set; } = null!;
        public long PermissionsMask { get; private set; } 
        public bool IsActive { get; private set; } = true;
        public int? EmployeeId { get; private set; }
        public Employee? Employee { get; private set; }
        private readonly List<UserLog> _userLogs = new List<UserLog>();
        public IReadOnlyCollection<UserLog> UserLogs => _userLogs.AsReadOnly();



        private User() { }



        private User(string userName, Password passwordHashing, long permissionsMask, int employeeId)
        {
            UserName = userName;
            UserPasswordHashing = passwordHashing;
            PermissionsMask = permissionsMask;
            EmployeeId = employeeId;
        }


        public static User Create(string userName, Password PasswordHashing, long PermissionsMask, int employeeId)
        {
            if(employeeId <= 0) throw new ArgumentNullException(nameof(employeeId));
            if(string.IsNullOrEmpty(userName)) throw new ArgumentException("UserName is required");
            if(PasswordHashing == null) throw new ArgumentNullException(nameof(PasswordHashing));

            return new User(userName, PasswordHashing, PermissionsMask, employeeId);
        }

        
        public User ChangeUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User name cannot be empty.", nameof(userName));

            UserName = userName.Trim();
            return this;
        }

        public User ChangePasswordHashing(Password passwordHashing)
        {
            if(passwordHashing == null) throw new ArgumentNullException(nameof(passwordHashing));


            UserPasswordHashing = passwordHashing;
            return this;
        }

       
        public Result<User> Freeze()
        {
            if (!IsActive)
                return Result<User>.Failure(UserIsNotActive);

            IsActive = false;
            return Result<User>.Successful(this);
        }

        public Result<User> Activate()
        {
            if (IsActive)
                return Result<User>.Failure(UserIsActive);

            IsActive = true;
            return Result<User>.Successful(this);
        }

        public Result<User> SetAdmin()
        {
            if (PermissionsMask == (long)enEmployeePermissions.Admin)
                return Result<User>.Failure(UserIsAdmin);

            PermissionsMask = (long)enEmployeePermissions.Admin;
            return Result<User>.Successful(this);
        }

        public User ChangePermissionsMask(long permissions)
        {
            PermissionsMask = permissions;
            return this;
        }


        public bool HasPermission(enEmployeePermissions permission)
        {
            if (PermissionsMask == (long)enEmployeePermissions.Admin) return true;
            return (PermissionsMask & (long)permission) == (long)permission;
        }
    }
}