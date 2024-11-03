using Common.Exceptions;

namespace Common.ResultPattern
{
    public static class Safe
    {
        private static string _reason = "An unmanaged exception occur and has been catch during the safe execution.";
        public static T Execute<T>(Func<T> funcToExecute)
        {
            try
            {
                return funcToExecute.Invoke();
            }
            catch (Exception e)
            {
                throw new SafeExecuteException(e.Message, funcToExecute.Method.Name, _reason, e.InnerException);
            }
        }

        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> funcToExecute)
        {
            try
            {
                return await funcToExecute.Invoke();
            }
            catch (Exception e)
            {
                throw new SafeExecuteException(e.Message, funcToExecute.Method.Name, _reason, e.InnerException);
            }
        }

        public static async Task ExecuteVoidAsync(Func<Task> funcToExecute)
        {
            try
            {
                await funcToExecute.Invoke();
            }
            catch (Exception e)
            {
                throw new SafeExecuteException(e.Message, funcToExecute.Method.Name, _reason, e.InnerException);
            }
        }
    }

    public static class Safe<TResult>
        where TResult : class
    {
        private static string _reason = "An unmanaged exception occur and has been catch during the safe execution.";

        public static async Task<Result<TResult>> ExecuteAsync(Func<Task<Result<TResult>>> funcToExecute)
        {
            try
            {
                return await funcToExecute();
            }
            catch (Exception e)
            {
                var safeExecException = new SafeExecuteException(e.Message, _reason, funcToExecute.Method.Name, e.InnerException);
                return new Result<TResult>(exception: safeExecException);
            }
        }

        public static async Task<Result> ExecuteAsync(Func<Task<Result>> funcToExecute)
        {
            try
            {
                return await funcToExecute();
            }
            catch (Exception e)
            {
                return new Result(exception: new SafeExecuteException(e.Message, _reason, funcToExecute.Method.Name, e.InnerException));
            }
        }

        // <summary>
        // Safely executes an action that returns a Result.
        // If any exception occurs during the action execution, an unexpected result is returned.
        // </summary>
        // <param name = "actionToSafelyExecute" > The action to safely execute.<//param>
        // <param name = "actionToExecuteIfException" > The optional action to execute if an exception occurs<//param>
        // <returns>The result returned by the action or Unexpected if an exception is thrown.</returns>
        public static Result SafeExecute(Func<Result> actionToSafelyExecute, Action<Exception> actionToExecuteIfException = null)
        {
            try
            {
                return actionToSafelyExecute();
            }
            catch (Exception exception)
            {
                actionToExecuteIfException?.Invoke(exception);
                return new Result(exception);
            }
        }

    }
}
