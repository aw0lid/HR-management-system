using SharedKernal;


namespace Application.ErrorsMenu
{
    public static class ApplicationErrorsMenu
    {
        public static class EmployeeHandlersErrors
        {
            public static Error DepartmentNotFound(int id) =>
                new("DEPARTMENT_NOT_FOUND", enErrorType.NotFound, [id.ToString()]);

            public static Error EmployeeNotFound(int id) =>
                new("EMPLOYEE_NOT_FOUND", enErrorType.NotFound, [id.ToString()]);

            public static Error ManagerNotFound(int id) =>
                new("MANAGER_NOT_FOUND", enErrorType.NotFound, [id.ToString()]);

            public static Error NationalityNotFound(int id) =>
                new("NATIONALITY_NOT_FOUND", enErrorType.NotFound, [id.ToString()]);

            public static Error JobTitleLevelNotFound(int id) =>
                new("JOBTITLELEVEL_NOT_FOUND", enErrorType.NotFound, [id.ToString()]);

            public static Error NationalNumberAlreadyExists(string nationalNumber) =>
                new("NATIONAL_NUMBER_ALREADY_EXISTS", enErrorType.Conflict, [nationalNumber]);

            public static Error EmailAlreadyExists(string email) =>
                new("EMAIL_ALREADY_EXISTS", enErrorType.Conflict, [email]);

            public static Error PhoneAlreadyExists(string phone) =>
                new("PHONE_NUMBER_ALREADY_EXISTS", enErrorType.Conflict, [phone]);
        }

        public static class UserHandlersErrors
        {
            public static Error UserNotFound(int id) =>
                new("USER_NOT_FOUND", enErrorType.NotFound, [id.ToString()]);

            public static Error UserLoginInvalid() =>
                new("USER_LOGIN_INVALID", enErrorType.NotFound);

            public static Error UserNotActive() =>
                new("USER_NOT_ACTIVE", enErrorType.Validation); 
            public static Error PermissionNotFound(int id) =>
                new("PERMISSION_NOT_FOUND", enErrorType.NotFound, [id.ToString()]);

            public static Error UserNameAlreadyExists(string username) =>
                new("USERNAME_ALREADY_EXISTS", enErrorType.Conflict, [username]);

            public static Error EmployeeNotHR(int employeeId) =>
                new("EMPLOYEE_NOT_HR", enErrorType.Conflict, [employeeId.ToString()]);

            public static Error EmployeeIsUser(int employeeId) =>
                new("EMPLOYEE_ALREADY_USER", enErrorType.Conflict, [employeeId.ToString()]);
        }


        public static class EmployeeQueriesErrors
        {
            public static Error EmployeeNotFound() =>
                new("EMPLOYEE_NOT_FOUND", enErrorType.NotFound);

            public static Error EmployeesEmpty() =>
                new("EMPLOYEES_EMPTY", enErrorType.NotFound);
        }

        public static class UsersQueriesErrors
        {
            public static Error UserNotFound() =>
                new("USER_NOT_FOUND", enErrorType.NotFound);

            public static Error UsersEmpty() =>
                new("USERS_EMPTY", enErrorType.NotFound);
        }

        public static class OrganizationsQueriesErrors
        {
            public static Error NationalitiesEmpty() => new("NATIONALITIES_EMPTY", enErrorType.NotFound);
            public static Error DepartmentsEmpty() => new("DEPARTMENTS_EMPTY", enErrorType.NotFound);
            public static Error JobTitlesEmpty() => new("JOB_TITLES_EMPTY", enErrorType.NotFound);
            public static Error JobGradesEmpty() => new("JOB_GRADES_EMPTY", enErrorType.NotFound);
            public static Error JobTitleLevelsEmpty() => new("JOB_TITLE_LEVELS_EMPTY", enErrorType.NotFound);
            public static Error PermissionsEmpty() => new("PERMISSIONS_EMPTY", enErrorType.NotFound);
        }
    }
}