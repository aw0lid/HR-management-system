using SharedKernal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using static Domain.ErrorsMenu.ErrorsMenu.Contacts;

namespace Domain.ValueObjects
{
    [ComplexType]
    public sealed record PhoneNumber
    {
        public string Value { get; init; }
        private PhoneNumber(string value) => Value = value;

        public static bool IsValid(string number) =>
            !string.IsNullOrEmpty(number) && Regex.IsMatch(number, @"^\+?[1-9]\d{1,14}$");


        public static Result<PhoneNumber> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Result<PhoneNumber>.Failure(InvalidPhone);

            if (!IsValid(value))
                return Result<PhoneNumber>.Failure(InvalidPhone);

            return Result<PhoneNumber>.Successful(new PhoneNumber(value));
        }
    }
}