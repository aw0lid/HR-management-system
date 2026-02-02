namespace Application.Write.Commands
{
    public record AdminFreezeCommand(int TargetUserId, string? reason);
    public record AdminReactivateCommand(int TargetUserId, string? reason);
    public record ResponseAdminActionCommand(int ActionId, int response, string? Notes);



    public record UserAddCommand(int EmployeeId, string userName, int role);
    public record UserUpdateCommand(string userName);
    public record LoginCommand(string userName, string password);
    public record ChangePasswordCommand(string OldPassword, string NewPassword);
    public record ResetPasswordCommand(string Username, string NewPassword, string Token);
    public record ActivateUserCommand(string Username, string NewPassword, string Token);
}