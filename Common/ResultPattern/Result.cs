using Common.Exceptions;

namespace Common.ResultPattern
{
    public class Result
    {
        public Result(){ }

        public Result(Exception exception)
        {
            Exception = exception;  
            ExceptionMessage = exception.Message;
            ReasonType = ReasonType.Unexpected;
        }

        public Exception Exception { get; set; }

        public string? ExceptionMessage { get; set; } = null;

        /// <summary>
        /// Filed used to explain why the code has failed
        /// </summary>
        public string? Reason { get; set; } = null;

        public ReasonType ReasonType { get; set; } = ReasonType.None;

        public bool IsSuccess { get => ReasonType == ReasonType.None && AllEmptyOrNull(Reason, ExceptionMessage); }
        
        public bool IsNotSuccess { get => !IsSuccess; }

        public Result AddReason(string reason)
        {
            Reason = reason;
            return this;
        }

        public Result AddType(ReasonType type)
        {
            ReasonType = type;
            return this;
        }

        public Result AddMessage(string message)
        {
            ExceptionMessage = message;
            return this;
        }

        public bool AllEmptyOrNull(string str1, string str2)
             => String.IsNullOrEmpty(str1) && String.IsNullOrEmpty(str2);
    }

    public class Result<T>
        where T : class
    {
        public Result()
        {
        }

        public Result(Exception exception)
        {
            SafeExecuteException = exception;
            ExceptionMessage = exception.Message;
        }

        public Result(T data)
        {
            Data = data;
        }

        #region public properties
        public T Data { get; set; } = null;

        public string? ExceptionMessage { get; set; } = null;

        /// <summary>
        /// Filed used to explain why the code has failed
        /// </summary>
        public string? Reason { get; set; } = null;

        public ReasonType ReasonType { get; set; } = ReasonType.None;

        public bool IsSuccess { get => ReasonType == ReasonType.None && AllEmptyOrNull(ExceptionMessage, Reason); }

        public bool IsNotSuccess { get => !IsSuccess; }

        public bool IsNotFound { get => IsSuccess && Data is null; }

        public bool IsFound { get => !IsNotFound; }

        public Exception SafeExecuteException { get; set; }

        #endregion public properties

        #region public Methods
        public Result<T> AddReason(string reason)
        {
            Reason = reason;
            return this;
        }

        public Result<T> AddType(ReasonType type)
        {
            ReasonType = type;
            return this;
        }

        public Result<T>AddMessage(string message)
        {
            ExceptionMessage = message;
            return this;
        }

        //TODO : This code is commented because this method is present in the Safe<T> Class
        //public static async Task<Result<T>> SafeExec(Func<Task<Result<T>>> actiontosafelyexecute)
        //{
        //    try
        //    {
        //        return await actiontosafelyexecute();
        //    }
        //    catch (Exception e)
        //    {
        //        var _reason = "an unmanaged exception occur and has been catch during the safe execution.";
        //        var safeexecexception = new SafeExecuteException(e.Message, _reason, actiontosafelyexecute.Method.Name, e.InnerException);
        //        return new Result<T>(exception: safeexecexception);
        //    }
        //}

        #endregion public Methods 
        public bool AllEmptyOrNull(string str1, string str2)
            => String.IsNullOrEmpty(str1) && String.IsNullOrEmpty(str2);
    }
}
