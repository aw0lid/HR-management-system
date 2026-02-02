namespace Application.Write.Commands
{
    public record UserAddCommand(int EmployeeId, string userName, string password, List<int>Permissions);
    public record UserUpdateCommand
    (
        string? userName, 
        string? password, 
        List<int>?AddPermissions, 
        List<int>?RemovePermissions
    );

    public record LoginCommand(string userName, string password);
    public record UserLoged(int Id, string userName, long PermissionMask);
}