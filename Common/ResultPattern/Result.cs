namespace Common.ResultPattern
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }
        public ReasonType ReasonType { get; }

        protected Result(bool isSuccess, string? error = null, ReasonType reasonType = ReasonType.None)
        {
            IsSuccess = isSuccess;
            Error = error;
            ReasonType = reasonType;
        }

        public static Result Success() => new(true);
        public static Result Failure(string error, ReasonType reasonType = ReasonType.Undefine)
            => new(false, error, reasonType);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        public Result(T data) : base(true)
        {
            Data = data;
        }

        public Result(string error, ReasonType reasonType)
            : base(false, error, reasonType) { }

        public static Result<T> Success(T value) => new(value);
        public static new Result<T> Failure(string error, ReasonType reasonType = ReasonType.Undefine)
            => new(error, reasonType);
    }

}
