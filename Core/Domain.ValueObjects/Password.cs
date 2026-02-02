using SharedKernal;
using static Domain.ErrorsMenu.ErrorsMenu.User;

namespace Domain.ValueObjects
{
    public sealed class Password
    {
        public string Value { get; private set; } = null!;

        public Password(string value) => Value = value;



        public static Result<Password> Create(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return Result<Password>.Failure(PasswordShortLength);

            if (!password.Any(char.IsUpper) || !password.Any(char.IsDigit))
                return Result<Password>.Failure(PasswordInvalid);

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            
            return Result<Password>.Successful(new Password(hashedPassword));
        }
    }
}