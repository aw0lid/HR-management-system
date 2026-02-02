using SharedKernal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using static Domain.ErrorsMenu.ErrorsMenu.Employee;


namespace Domain.ValueObjects
{
    [ComplexType]
    public sealed record NationalNumber
    {
        public string Value { get; init; }
        private NationalNumber(string value) => Value = value;


        public static bool IsValid(string nationalNumber) =>
            !string.IsNullOrWhiteSpace(nationalNumber) && Regex.IsMatch(nationalNumber, @"^\d{14}$");


        public static Result<NationalNumber> Create(string nationalNumber)
        {
            if (string.IsNullOrWhiteSpace(nationalNumber) || !IsValid(nationalNumber))
                return Result<NationalNumber>.Failure(InvalidNationalNumber);

            return Result<NationalNumber>.Successful(new NationalNumber(nationalNumber));
        }
    }
}