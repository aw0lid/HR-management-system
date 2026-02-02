using Application.Write.Commands;
using FluentValidation;

namespace Application.Write.FluentValidation
{
    public static class ValidationKeys
    {
        public const string Required = "FIELD_REQUIRED";
        public const string TooLong = "FIELD_TOO_LONG";
        public const string TooShort = "FIELD_TOO_SHORT";
        public const string InvalidFormat = "INVALID_FORMAT";
        public const string InvalidSelection = "INVALID_SELECTION";
        public const string AtLeastOneField = "AT_LEAST_ONE_FIELD_REQUIRED";
        public const string InvalidRange = "VALUE_OUT_OF_RANGE";
    }   
}