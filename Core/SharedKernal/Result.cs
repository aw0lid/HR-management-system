
namespace SharedKernal
{

    public enum enErrorType
    {
        Failure = 1,
        Validation = 2,
        NotFound = 3,
        Conflict = 4,
        Unauthorized = 5
    }

    public readonly struct Result<TValue>
    {
        public readonly TValue? Value;
        public readonly Error? Error;

        public bool IsSuccess { get; }
       

        private Result(TValue value)
        {
            IsSuccess = true;
            Value = value;
            Error = default;
        }

        private Result(Error error)
        {
            Error = error;
            IsSuccess = false;
            Value = default;
        }

        

        public static Result<TValue> Successful(TValue value) => new(value);
        public static Result<TValue> Failure(Error? error = default) => new(error!);
    }

  
    public record Error(string Key, enErrorType Type, string[]? Args = null);
}