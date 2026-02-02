namespace Application.Write.Commands
{
    public record EmployeeAddCommand 
    (
        string EmployeeFirstName,
        string EmployeeSecondName,
        string EmployeeThirdName,
        string EmployeeLastName,
        int EmployeeNationalityId,
        DateTime EmployeeBirthDate,
        string EmployeeNationalNumber,
        short EmployeeGender,
        int? ManagerId,
        int DepartmentId,
        int JobTitleLevelId,
        string PhoneNumber,
        string Email,
        string Address
    );




    public record EmployeePersonalInfoUpdateCommand
    (
        string? EmployeeFirstName,
        string? EmployeeSecondName,
        string? EmployeeThirdName,
        string? EmployeeLastName,
        string? EmployeeNationalNumber,
        int? EmployeeNationalityId,
        DateTime? EmployeeBirthDate = null,
        short? EmployeeGender = null,
        string? Address = null
    );



    public record EmployeeWorkInfoUpdateCommand 
    (
        int? ManagerId = null,
        int? DepartmentId = null,
        int? JobTitleLevelId = null
    );
}