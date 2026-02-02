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
            public static Error UserNotFound(int userId) =>
                new("USER_NOT_FOUND", enErrorType.NotFound, [userId.ToString()]);

            public static Error UserNotFound(string userName) =>
                new("USER_NOT_FOUND", enErrorType.NotFound, [userName]);

            public static Error UserLoginInvalid() =>
                new("USER_LOGIN_INVALID", enErrorType.NotFound);

            public static Error UserNotAdmin(int userId) =>
                new("USER_NOT_ADMIN", enErrorType.Unauthorized, [userId.ToString()]);

             public static Error UserNotAdmin(string userName) =>
                new("USER_NOT_ADMIN", enErrorType.Unauthorized, [userName]);

            public static Error UserNotActive(int userId) =>
                new("USER_NOT_ACTIVE", enErrorType.Validation, [userId.ToString()]);
                
            public static Error UserNotActive(string userName) =>
                new("USER_NOT_ACTIVE", enErrorType.Validation, [userName]);

            public static Error UserActive(int userId) =>
                new("USER_ACTIVE", enErrorType.Validation, [userId.ToString()]);
                
            public static Error UserActive(string userName) =>
                new("USER_ACTIVE", enErrorType.Validation, [userName]);

            public static Error CannotPerformActionOnSelf() =>
                new("CANNOT_PERFORM_ACTION_ON_SELF", enErrorType.Validation);

            public static Error AdminTargetProcessHisAction() =>
                new("ADMIN_TARGET_PROCESS_ACTION", enErrorType.Validation);

            public static Error CannotFreezeLastAdmins() =>
                new("CANNOT_FREEZE_LAST_ADMINS", enErrorType.Validation);

            public static Error TargetHasAnotherPendingAction(int userId) =>
                new("ADMIN_HAS_ANOTHER_PENDING_ACTION", enErrorType.Validation, [userId.ToString()]);
            
            public static Error InvalidUserName() => 
                new("INVALID_USERNAME", enErrorType.Validation);

            public static Error PasswordNotMatch() => 
                new("PASSWORD_NOT_MATCH", enErrorType.Validation);

            public static Error UserNameAlreadyExists(string username) =>
                new("USERNAME_ALREADY_EXISTS", enErrorType.Conflict, [username]);

            public static Error EmployeeNotHR(int employeeId) =>
                new("EMPLOYEE_NOT_HR", enErrorType.Conflict, [employeeId.ToString()]);

            public static Error EmployeeNotSystemAdmin(int employeeId) =>
                new("EMPLOYEE_NOT_SYSTEM_ADMIN", enErrorType.Conflict, [employeeId.ToString()]);

            public static Error EmployeeIsUser(int employeeId) =>
                new("EMPLOYEE_ALREADY_USER", enErrorType.Conflict, [employeeId.ToString()]);


            public static Error PendingActionNotFound(int actionId) =>
                new("PENDING_ACTION_NOT_FOUND", enErrorType.NotFound, [actionId.ToString()]);

            public static Error PendingActionAlreadyProcessed(int ActionId) =>
                new("PENDING_ACTION_ALREADY_PROCESSED", enErrorType.Validation);

            public static Error AdminApproveHisAction() =>
                new("ADMIN_APPROVE_HIS_ACTION", enErrorType.Validation);

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

            public static Error PendingAdminsActionsEmpty() => 
                new("PENDING_ADMINS_ACTIONS_EMPTY", enErrorType.NotFound);

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