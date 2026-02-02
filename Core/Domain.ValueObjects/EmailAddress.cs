using SharedKernal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using static Domain.ErrorsMenu.ErrorsMenu.Contacts;

namespace Domain.ValueObjects
{
    [ComplexType]
    public sealed record EmailAddress
    {
        public string Value { get; init; }
        private EmailAddress(string value) => Value = value;

        public static bool IsValid(string email) =>
            !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");


        public static Result<EmailAddress> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<EmailAddress>.Failure(InvalidEmail);


            if (!IsValid(email))
                return Result<EmailAddress>.Failure(InvalidEmail);

            return Result<EmailAddress>.Successful(new EmailAddress(email));
        }
    }
}