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
            public static Error UserIsNotActive = new("USR_NOT_ACTIVE", enErrorType.Validation);
            public static Error UserIsActive = new("USR_ALREADY_ACTIVE", enErrorType.Validation);
            public static Error ForbiddenRoleChange = new("USR_FORBIDDEN_ROLE_CHANGE", enErrorType.Validation);
            public static Error PasswordShortLength = new("USR_PASS_TOO_SHORT", enErrorType.Validation);
            public static Error PasswordInvalid = new("USR_PASS_INVALID", enErrorType.Validation);
            public static Error ActionNotPending = new("ACT_NOT_PENDING", enErrorType.Validation);
            public static Error CannotApproveOwnAction = new("ACT_SELF_APPROVAL", enErrorType.Validation);
            public static Error InvalidProcessorId = new("ACT_INVALID_PROCESSOR", enErrorType.Validation);
        }   
    }
}