using SharedKernal;


namespace Domain.ErrorsMenu
{
    public static class ErrorsMenu
    {
        public static class Employee
        {
            public static readonly Error InvalidNationalNumber = new("EMP_NATIONAL_NUMBER_INVALID", enErrorType.Validation);
            public static readonly Error UnderAge = new("EMP_AGE_LESS_THAN_18", enErrorType.Validation);
            public static readonly Error OverAge = new("EMP_AGE_GREATER_THAN_65", enErrorType.Validation);
            public static readonly Error NotActive = new("EMP_STATUS_NOT_ACTIVE", enErrorType.Failure);
            public static readonly Error Terminated = new("EMP_STATUS_TERMINATED", enErrorType.Failure);
            public static readonly Error Resigned = new("EMP_STATUS_RESIGNED", enErrorType.Failure);
            public static readonly Error Retired = new("EMP_STATUS_RETIRED", enErrorType.Failure);
            public static readonly Error Suspended = new("EMP_STATUS_SUSPENDED", enErrorType.Failure);
            public static readonly Error OnLeave = new("EMP_STATUS_ON_LEAVE", enErrorType.Failure);
            public static readonly Error WorkInfoIsNotCurrent = new("EMP_WORKINFO_FROZEN", enErrorType.Failure);
            public static readonly Error PhoneDuplicate = new("EMP_PHONE_ALREADY_EXISTS", enErrorType.Conflict);
            public static readonly Error EmailDuplicate = new("EMP_EMAIL_ALREADY_EXISTS", enErrorType.Conflict);
            public static readonly Error ManagerNotActive = new("EMP_MGR_NOT_ACTIVE", enErrorType.Failure);
            public static readonly Error DifferentDepartment = new("EMP_MGR_DEPT_MISMATCH", enErrorType.Failure);
            public static readonly Error ManagerLowerGrade = new("EMP_MGR_GRADE_TOO_LOW", enErrorType.Failure);
            public static readonly Error CircularReference = new("EMP_MGR_CIRCULAR_REF", enErrorType.Failure);
            public static readonly Error NationalityNameEmpty = new("NAT_NAME_REQUIRED", enErrorType.Validation);
            public static readonly Error PhoneNotFound = new("EMP_PHONE_NOT_FOUND", enErrorType.NotFound);
            public static readonly Error EmailNotFound = new("EMP_EMAIL_NOT_FOUND", enErrorType.NotFound);
        }

        public static class Contacts
        {
            public static Error InvalidPhone = new("CON_PHONE_INVALID", enErrorType.Validation);
            public static Error CannotFreezePrimaryPhone = new("CON_PHONE_PRIMARY_FREEZE_DENIED", enErrorType.Validation);
            public static Error CannotSetInactivePhoneAsPrimary = new("CON_PHONE_INACTIVE_SET_PRIMARY_DENIED", enErrorType.Validation);
            public static Error IsNotPrimaryPhone = new("CON_PHONE_NOT_PRIMARY", enErrorType.Validation);
            public static Error InvalidEmail = new("CON_EMAIL_INVALID", enErrorType.Validation);
            public static Error CannotFreezePrimaryEmail = new("CON_EMAIL_PRIMARY_FREEZE_DENIED", enErrorType.Validation);
            public static Error CannotSetInactiveEmailAsPrimary = new("CON_EMAIL_INACTIVE_SET_PRIMARY_DENIED", enErrorType.Validation);
            public static Error IsNotPrimaryEmail = new("CON_EMAIL_NOT_PRIMARY", enErrorType.Validation); 
        }

        public static class User
        {
            public static Error UserIsNotActive = new("USE_NOT_ACTIVE", enErrorType.Validation);
            public static Error UserIsActive = new("USE_ACTIVE", enErrorType.Validation);
            public static Error UserIsAdmin = new("USE_IS_ADMIN", enErrorType.Validation);
            public static Error UserIsNotAdmin = new("USE_NOT_ADMIN", enErrorType.Validation);
            public static Error PasswordShortLength = new("PASS_SHORT_LEN", enErrorType.Validation);
            public static Error PasswordInvalid = new("PASS_INVALID", enErrorType.Validation);
            public static Error PermissionsInvalid = new("PERMISSIONS_INVALID", enErrorType.Validation);
            public static Error UserHasPermissions(int Id) => new("USER_ALREADY_HAS_PERMISSION", enErrorType.Validation, [Id.ToString()]);
            public static Error UserHasNotPermissions(int Id) => new("USER_ALREADY_HASNOT_PERMISSION", enErrorType.Validation, [Id.ToString()]);
        }
    }
}