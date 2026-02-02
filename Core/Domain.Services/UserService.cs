using Domain.Entites;
using Domain.ValueObjects;
using SharedKernal;
using static Domain.ErrorsMenu.ErrorsMenu.User;

namespace Domain.Services
{
    public class UserService
    {
        private long PermissionsMaskCalculater(List<Permission> Permissions) => Permissions.Aggregate(0L, (currentMask, p) => currentMask | p.BitValue);
        private bool UserHasPermission(Permission permission, User user) => (user.PermissionsMask & permission.BitValue) == permission.BitValue;
        private User ChangeMask(User user, List<Permission> permissions, bool add)
        {
            long targetMask = permissions.Aggregate(0L, (current, p) => current | p.BitValue);

            if (add) return user.ChangePermissionsMask(user.PermissionsMask | targetMask);
            else return user.ChangePermissionsMask(user.PermissionsMask & ~targetMask);
        }



        public Result<User> AddUseCase(int EmployeeId, string userName, Password password, List<Permission> Permissions)
        {
            long mask = PermissionsMaskCalculater(Permissions);
           if(!Permission.IsValidPermissionMask(mask)) return Result<User>.Failure(PermissionsInvalid);

            var user = User.Create(userName, password, mask, EmployeeId);
            return Result<User>.Successful(user);
        }

        public Result<User> AddNewPermissionsCase(User user, List<Permission> NewPermissions)
        {
            foreach(var per in NewPermissions)
            {
                if(UserHasPermission(per, user))
                    return Result<User>.Failure(UserHasPermissions(per.PermissionId));
            }

            return Result<User>.Successful(ChangeMask(user, NewPermissions, true));
        }

        public Result<User> RemovePermissionsCase(User user, List<Permission> Permissions)
        {
            foreach(var per in Permissions)
            {
                if(!UserHasPermission(per, user))
                    return Result<User>.Failure(UserHasNotPermissions(per.PermissionId));
            }

            return Result<User>.Successful(ChangeMask(user, Permissions, false));
        }
    }
}