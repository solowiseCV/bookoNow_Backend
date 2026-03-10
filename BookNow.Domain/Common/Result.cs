namespace BookNow.Domain.Common
{
    // Generic result container used across the application for operations that can succeed or fail.
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public T? Data { get; private set; }
        public IReadOnlyList<string> Errors => _errors.AsReadOnly();

        private readonly List<string> _errors = new List<string>();

        private Result() { }

        public static Result<T> Success(T data, string message = "")
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static Result<T> Failure(string message, IEnumerable<string>? errors = null)
        {
            var result = new Result<T>
            {
                IsSuccess = false,
                Message = message
            };

            if (errors != null)
            {
                result._errors.AddRange(errors);
            }

            return result;
        }
    }
}